using System;
using UnityEngine;

/// <summary>
/// Macin kurallarini isleten state machine. UI'i tanimaz: sadece olay yayinlar,
/// dinlemek isteyen abone olur. Boylece top ve oyuncu da UI'dan habersiz kalir.
/// </summary>
public class MatchManager : MonoBehaviour
{
    [SerializeField] private MatchSettings settings;

    [Header("Sahne referanslari (bos birakilirsa otomatik bulunur)")]
    [SerializeField] private BallController ball;
    [SerializeField] private PlayerController[] players;
    [SerializeField] private GoalTrigger[] goals;

    private MatchTimer timer;
    private readonly MatchScore score = new MatchScore();

    private float stateTimer;
    private int lastShownSecond = -1;

    private Vector2 ballSpawn;
    private Vector2[] playerSpawns;

    public MatchState State { get; private set; }
    public MatchScore Score => score;
    public float TimeRemaining => timer == null ? 0f : timer.Remaining;

    public event Action<int, int> ScoreChanged;
    public event Action<float> TimeChanged;
    public event Action<MatchState> StateChanged;
    public event Action<Side> GoalScored;
    public event Action MatchEnded;

    void Awake()
    {
        // Top bir prefab ornegi; sahneden prefab icine referans vermek zahmetli
        // oldugu icin bos birakilan alanlari basta bir kez kendimiz buluyoruz.
        if (ball == null) ball = FindObjectOfType<BallController>();
        if (players == null || players.Length == 0) players = FindObjectsOfType<PlayerController>();
        if (goals == null || goals.Length == 0) goals = FindObjectsOfType<GoalTrigger>();

        timer = new MatchTimer(settings.matchDuration);

        // Sahnedeki baslangic yerlesimi = kickoff yerlesimi.
        ballSpawn = ball.transform.position;
        playerSpawns = new Vector2[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerSpawns[i] = players[i].transform.position;
        }
    }

    void OnEnable()
    {
        if (goals == null) return;

        foreach (GoalTrigger goal in goals)
        {
            goal.Conceded += HandleConceded;
        }
    }

    void OnDisable()
    {
        if (goals == null) return;

        foreach (GoalTrigger goal in goals)
        {
            goal.Conceded -= HandleConceded;
        }
    }

    void Start()
    {
        RestartMatch();
    }

    void Update()
    {
        switch (State)
        {
            case MatchState.KickOff:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) EnterState(MatchState.Playing);
                break;

            case MatchState.Playing:
                bool finished = timer.Tick(Time.deltaTime);
                PublishTimeIfSecondChanged();
                if (finished) EnterState(MatchState.MatchEnd);
                break;

            case MatchState.GoalScored:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f) EnterState(MatchState.KickOff);
                break;
        }
    }

    public void RestartMatch()
    {
        score.Reset();
        timer.Reset();
        lastShownSecond = -1;

        ScoreChanged?.Invoke(score.Left, score.Right);
        PublishTimeIfSecondChanged();

        EnterState(MatchState.KickOff);
    }

    private void HandleConceded(Side defendingSide)
    {
        // Kutlama sirasinda top hala kalede duruyor olabilir; ikinci golu saymayalim.
        if (State != MatchState.Playing) return;

        Side scorer = defendingSide.Opposite();
        score.Add(scorer);

        ScoreChanged?.Invoke(score.Left, score.Right);
        GoalScored?.Invoke(scorer);

        EnterState(MatchState.GoalScored);
    }

    private void EnterState(MatchState next)
    {
        State = next;

        switch (next)
        {
            case MatchState.KickOff:
                ResetPositions();
                SetPlayEnabled(false);
                stateTimer = settings.kickOffDelay;
                break;

            case MatchState.Playing:
                SetPlayEnabled(true);
                break;

            case MatchState.GoalScored:
                SetPlayEnabled(false);
                stateTimer = settings.goalCelebrationDuration;
                break;

            case MatchState.MatchEnd:
                SetPlayEnabled(false);
                MatchEnded?.Invoke();
                break;
        }

        StateChanged?.Invoke(next);
    }

    private void ResetPositions()
    {
        ball.ResetTo(ballSpawn);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetTo(playerSpawns[i]);
        }
    }

    private void SetPlayEnabled(bool enabled)
    {
        // Time.timeScale kullanmiyoruz: durdurmak sadece oynanisi ilgilendirir,
        // UI animasyonlari ve geri sayim akmaya devam etmeli.
        ball.SetSimulated(enabled);

        foreach (PlayerController player in players)
        {
            player.SetInputEnabled(enabled);
        }
    }

    /// <summary>
    /// Sureyi her karede degil, saniye degistikce yayinlar. Aksi halde UI her
    /// karede yeni bir string ureterek bos yere GC baskisi yaratir.
    /// </summary>
    private void PublishTimeIfSecondChanged()
    {
        int second = Mathf.CeilToInt(timer.Remaining);
        if (second == lastShownSecond) return;

        lastShownSecond = second;
        TimeChanged?.Invoke(timer.Remaining);
    }
}
