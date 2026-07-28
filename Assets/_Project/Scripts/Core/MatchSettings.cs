using UnityEngine;

[CreateAssetMenu(fileName = "MatchSettings", menuName = "Game/Match Settings")]
public class MatchSettings : ScriptableObject
{
    [Tooltip("Mac suresi (saniye)")]
    public float matchDuration = 90f;

    [Tooltip("Baslangicta ve gol sonrasi beklenen geri sayim")]
    public float kickOffDelay = 2f;

    [Tooltip("Gol sonrasi kutlama suresi")]
    public float goalCelebrationDuration = 1.5f;
}
