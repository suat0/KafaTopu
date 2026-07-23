using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private BallController ballController;

    public void Show()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        finalScoreText.text = scoreData.GetScore().ToString();
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        scoreData.ResetScore();
        gameOverPanel.SetActive(false);

        ballController.ResetBall();
    }
}