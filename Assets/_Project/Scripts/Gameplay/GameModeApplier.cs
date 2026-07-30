using UnityEngine;

/// <summary>
/// Menude secilen modu sahneye uygular: ikinci oyuncuyu AI mi insan mi surecek.
/// </summary>
public class GameModeApplier : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private PlayerController secondPlayer;
    [SerializeField] private AIInputSource aiSource;
    [SerializeField] private KeyboardInputSource keyboardSource;

    void Awake()
    {
        bool useAI = !settings.twoPlayers;

        aiSource.enabled = useAI;
        keyboardSource.enabled = !useAI;

        if (useAI && settings.CurrentDifficulty != null)
        {
            aiSource.SetSettings(settings.CurrentDifficulty);
        }

        // PlayerController kaynagini Awake'te secmisti; bileseni degistirdik, yeniden sectir.
        secondPlayer.RefreshInputSource();
    }
}
