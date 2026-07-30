using UnityEngine;

public class AIInputSource : MonoBehaviour, IInputSource
{
    [SerializeField] private AISettings settings;

    [Tooltip("Bos birakilirsa sahnedeki top otomatik bulunur")]
    [SerializeField] private Transform ball;

    public float Horizontal { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }

    private Rigidbody2D ballBody;
    private float nextDecisionTime;
    private float targetX;
    private bool jumpHeldLastFrame;

    void Awake()
    {
        if (ball == null)
        {
            BallController found = FindObjectOfType<BallController>();
            if (found != null) ball = found.transform;
        }

        if (ball != null) ballBody = ball.GetComponent<Rigidbody2D>();
    }

    /// <summary>Zorluk secimi menuden geldigi icin calisma aninda degistirilebilmeli.</summary>
    public void SetSettings(AISettings value)
    {
        if (value != null) settings = value;
    }

    public void Tick()
    {
        if (ball == null)
        {
            Horizontal = 0f;
            JumpPressed = false;
            JumpReleased = false;
            return;
        }

        // Reaksiyon gecikmesi: hedef her karede degil, araliklarla yenilenir.
        if (Time.time >= nextDecisionTime)
        {
            nextDecisionTime = Time.time + settings.reactionDelay;
            UpdateTarget();
        }

        Horizontal = AIBrain.DecideHorizontal(transform.position.x, targetX, settings.moveDeadzone);

        bool wantsJump = AIBrain.DecideJump(transform.position.x, transform.position.y,
                                            ball.position.x, ball.position.y,
                                            settings.jumpRange, settings.jumpMinHeight);

        // Insan oyuncunun tus basisini taklit et: basildi/birakildi kenarlari
        JumpPressed = wantsJump && !jumpHeldLastFrame;
        JumpReleased = !wantsJump && jumpHeldLastFrame;
        jumpHeldLastFrame = wantsJump;
    }

    private void UpdateTarget()
    {
        Vector2 ballPosition = ball.position;
        Vector2 ballVelocity = ballBody != null ? ballBody.velocity : Vector2.zero;
        float gravity = ballBody != null ? -Physics2D.gravity.y * ballBody.gravityScale : 9.81f;

        float predicted = AIBrain.PredictLandingX(ballPosition.x, ballPosition.y,
                                                  ballVelocity.x, ballVelocity.y,
                                                  transform.position.y, gravity);

        targetX = predicted + Random.Range(-settings.aimError, settings.aimError);
    }
}
