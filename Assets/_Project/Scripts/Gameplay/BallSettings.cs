using UnityEngine;

[CreateAssetMenu(fileName = "BallSettings", menuName = "Game/Ball Settings")]
public class BallSettings : ScriptableObject
{
    [Header("Hiz Sinirlari")]
    [Tooltip("Topun ulasabilecegi en yuksek hiz")]
    public float maxSpeed = 18f;

    [Tooltip("Topun en yuksek donus hizi (derece/sn)")]
    public float maxSpin = 720f;

    [Header("Vurus")]
    [Tooltip("Her temasta topa eklenen temel itme")]
    public float kickImpulse = 3f;

    [Tooltip("Oyuncunun hizinin topa aktarilan orani")]
    [Range(0f, 1f)] public float playerVelocityTransfer = 0.35f;

    [Tooltip("Yatay vurusta topa verilen donus")]
    public float spinPerKick = 120f;
}
