using UnityEngine;
using TMPro;

public class GoalAnnouncementUI : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private GameObject goalText;
    [SerializeField] private TextMeshProUGUI label;

    void OnEnable()
    {
        matchManager.GoalScored += Show;
        matchManager.StateChanged += HandleStateChanged;
        goalText.SetActive(false);
    }

    void OnDisable()
    {
        matchManager.GoalScored -= Show;
        matchManager.StateChanged -= HandleStateChanged;
    }

    private void Show(Side scorer)
    {
        label.text = scorer == Side.Left ? "GOL!  SOL" : "GOL!  SAG";
        goalText.SetActive(true);
    }

    private void HandleStateChanged(MatchState state)
    {
        if (state != MatchState.GoalScored) goalText.SetActive(false);
    }
}
