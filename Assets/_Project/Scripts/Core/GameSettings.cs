using UnityEngine;

/// <summary>
/// Menude secilen ve oyun sahnesine tasinan tercihler.
///
/// Burada ScriptableObject kullanmak bilincli: sahne degistiginde MonoBehaviour'lar
/// yok olur, asset ise ayakta kalir. MatchScore'da SO'dan kacinmistik cunku orada
/// kaliciligi *istemiyorduk*; burada tam tersi gerekiyor.
/// </summary>
[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/Game Settings")]
public class GameSettings : ScriptableObject
{
    public bool twoPlayers;

    [Tooltip("difficulties dizisindeki sira")]
    public int difficultyIndex = 1;

    [Tooltip("Kolay, Normal, Zor")]
    public AISettings[] difficulties;

    public AISettings CurrentDifficulty
    {
        get
        {
            if (difficulties == null || difficulties.Length == 0) return null;
            return difficulties[Mathf.Clamp(difficultyIndex, 0, difficulties.Length - 1)];
        }
    }

    public void CycleDifficulty()
    {
        if (difficulties == null || difficulties.Length == 0) return;
        difficultyIndex = (difficultyIndex + 1) % difficulties.Length;
    }
}
