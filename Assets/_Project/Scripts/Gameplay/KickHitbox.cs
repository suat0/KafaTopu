using UnityEngine;

/// <summary>
/// Bu collider'a carpan topun ne kadar sert itilecegini belirler.
/// Bileseni olmayan collider'lar carpani 1 kabul edilir.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class KickHitbox : MonoBehaviour
{
    [SerializeField] private float kickMultiplier = 1.6f;

    public float KickMultiplier => kickMultiplier;
}
