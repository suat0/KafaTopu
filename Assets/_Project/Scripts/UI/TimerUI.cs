using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private TextMeshProUGUI timerText;

    void OnEnable()
    {
        matchManager.TimeChanged += Render;
        Render(matchManager.TimeRemaining);
    }

    void OnDisable()
    {
        matchManager.TimeChanged -= Render;
    }

    private void Render(float remaining)
    {
        int total = Mathf.CeilToInt(remaining);
        timerText.text = (total / 60) + ":" + (total % 60).ToString("00");
    }
}
