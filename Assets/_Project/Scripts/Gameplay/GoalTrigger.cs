using System;
using UnityEngine;

/// <summary>
/// Kale agzindaki tetikleyici. Kural bilmez, sadece "bu kaleye top girdi" der;
/// karari MatchManager verir.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GoalTrigger : MonoBehaviour
{
    [Tooltip("Bu kaleyi hangi taraf savunuyor")]
    [SerializeField] private Side defendingSide;

    [SerializeField] private LayerMask ballLayer;

    /// <summary>Gol yiyen tarafi bildirir.</summary>
    public event Action<Side> Conceded;

    public Side DefendingSide => defendingSide;

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((ballLayer.value & (1 << other.gameObject.layer)) == 0) return;

        Conceded?.Invoke(defendingSide);
    }
}
