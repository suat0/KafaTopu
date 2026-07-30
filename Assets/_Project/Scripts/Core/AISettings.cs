using UnityEngine;

[CreateAssetMenu(fileName = "AISettings", menuName = "Game/AI Settings")]
public class AISettings : ScriptableObject
{
    [Tooltip("Menude gorunen ad")]
    public string displayName = "Normal";

    [Tooltip("Hedefini bu araliklarla gunceller - buyudukce AI gec kalir")]
    public float reactionDelay = 0.15f;

    [Tooltip("Tahmine eklenen rastgele sapma. Zorluk = daha az hata, daha hizli degil")]
    public float aimError = 0.6f;

    [Tooltip("Hedefe bu kadar yakinsa yerinde durur")]
    public float moveDeadzone = 0.35f;

    [Tooltip("Top yatayda bu mesafedeyken ziplamayi dener")]
    public float jumpRange = 1.8f;

    [Tooltip("Top, oyuncunun bu kadar ustundeyse ziplar")]
    public float jumpMinHeight = 0.3f;
}
