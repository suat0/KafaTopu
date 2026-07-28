using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Canvas uzerinde durur, panelin kendisinde degil: panel kapaliyken de
/// olaylari dinleyebilmesi gerekiyor.
/// </summary>
public class MatchEndUI : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button restartButton;

    void OnEnable()
    {
        matchManager.MatchEnded += Show;
        matchManager.StateChanged += HandleStateChanged;

        // Inspector'dan baglanan onClick'ler refactor'da sessizce kopar - koddan bagla.
        restartButton.onClick.AddListener(Restart);

        panel.SetActive(false);
    }

    void OnDisable()
    {
        matchManager.MatchEnded -= Show;
        matchManager.StateChanged -= HandleStateChanged;
        restartButton.onClick.RemoveListener(Restart);
    }

    private void HandleStateChanged(MatchState state)
    {
        if (state != MatchState.MatchEnd)
        {
            panel.SetActive(false);
        }
    }

    private void Show()
    {
        MatchScore score = matchManager.Score;

        string headline = score.IsDraw
            ? "Berabere"
            : score.Winner == Side.Left ? "Sol Kazandi" : "Sag Kazandi";

        resultText.text = headline + "\n" + score.Left + " - " + score.Right;

        panel.SetActive(true);
    }

    private void Restart()
    {
        matchManager.RestartMatch();
    }
}
