using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameSettings settings;

    [SerializeField] private Button onePlayerButton;
    [SerializeField] private Button twoPlayerButton;
    [SerializeField] private Button difficultyButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI difficultyLabel;

    void OnEnable()
    {
        onePlayerButton.onClick.AddListener(StartOnePlayer);
        twoPlayerButton.onClick.AddListener(StartTwoPlayer);
        difficultyButton.onClick.AddListener(CycleDifficulty);
        quitButton.onClick.AddListener(Quit);

        RenderDifficulty();
    }

    void OnDisable()
    {
        onePlayerButton.onClick.RemoveListener(StartOnePlayer);
        twoPlayerButton.onClick.RemoveListener(StartTwoPlayer);
        difficultyButton.onClick.RemoveListener(CycleDifficulty);
        quitButton.onClick.RemoveListener(Quit);
    }

    private void StartOnePlayer()
    {
        settings.twoPlayers = false;
        SceneManager.LoadScene(SceneNames.Game);
    }

    private void StartTwoPlayer()
    {
        settings.twoPlayers = true;
        SceneManager.LoadScene(SceneNames.Game);
    }

    private void CycleDifficulty()
    {
        settings.CycleDifficulty();
        RenderDifficulty();
    }

    private void RenderDifficulty()
    {
        AISettings current = settings.CurrentDifficulty;
        difficultyLabel.text = current == null ? "Zorluk: -" : "Zorluk: " + current.displayName;
    }

    private void Quit()
    {
        // Editorde hicbir sey yapmaz; yalnizca build'de calisir.
        Application.Quit();
    }
}
