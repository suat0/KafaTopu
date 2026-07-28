using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private MatchManager matchManager;
    [SerializeField] private TextMeshProUGUI scoreText;

    void OnEnable()
    {
        matchManager.ScoreChanged += Render;
        Render(matchManager.Score.Left, matchManager.Score.Right);
    }

    void OnDisable()
    {
        matchManager.ScoreChanged -= Render;
    }

    // Her karede degil, yalnizca skor degistiginde calisir.
    private void Render(int left, int right)
    {
        scoreText.text = left + " - " + right;
    }
}
