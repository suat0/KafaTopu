using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    void OnEnable()
    {
        matchManager.StateChanged += Render;

        resumeButton.onClick.AddListener(Resume);
        restartButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(GoToMenu);

        Render(matchManager.State);
    }

    void OnDisable()
    {
        matchManager.StateChanged -= Render;

        resumeButton.onClick.RemoveListener(Resume);
        restartButton.onClick.RemoveListener(Restart);
        menuButton.onClick.RemoveListener(GoToMenu);
    }

    private void Render(MatchState state)
    {
        pausePanel.SetActive(state == MatchState.Paused);
    }

    private void Resume()
    {
        matchManager.TogglePause();
    }

    private void Restart()
    {
        // Once duraklatmadan cik, yoksa mac durmus halde yeniden baslar
        matchManager.TogglePause();
        matchManager.RestartMatch();
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene(SceneNames.MainMenu);
    }
}
