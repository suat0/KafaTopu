using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private GameObject pausePanel;

    void OnEnable()
    {
        matchManager.StateChanged += Render;
        Render(matchManager.State);
    }

    void OnDisable()
    {
        matchManager.StateChanged -= Render;
    }

    private void Render(MatchState state)
    {
        pausePanel.SetActive(state == MatchState.Paused);
    }
}
