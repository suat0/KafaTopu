using UnityEngine;
using TMPro;

/// <summary>
/// Kickoff geri sayimini gosterir. Canvas uzerinde durur; yonettigi yazi
/// kapaliyken de olaylari dinleyebilmesi gerekiyor.
/// </summary>
public class CountdownUI : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private TextMeshProUGUI countdownText;

    void OnEnable()
    {
        matchManager.CountdownChanged += Render;
        Render(0);
    }

    void OnDisable()
    {
        matchManager.CountdownChanged -= Render;
    }

    private void Render(int seconds)
    {
        countdownText.gameObject.SetActive(seconds > 0);

        if (seconds > 0)
        {
            countdownText.text = seconds.ToString();
        }
    }
}
