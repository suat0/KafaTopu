using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private TextMeshProUGUI scoreText;

    void Update()
    {
        scoreText.text = scoreData.GetScore().ToString();
    }
}