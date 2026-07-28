using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] private BallSettings settings;
    [SerializeField] private LayerMask playerLayer;

    [Tooltip("Top bir sekilde sahadan kacarsa buraya dusunce basa doner")]
    [SerializeField] private float fallLimitY = -8f;

    private Rigidbody2D rb;
    private Vector2 lastSpawn;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastSpawn = transform.position;
    }

    void FixedUpdate()
    {
        if (!rb.simulated) return;

        // Sinirsiz hizlanan top collider'lari atlar ve oynanamaz hale gelir
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, settings.maxSpeed);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -settings.maxSpin, settings.maxSpin);

        // Guvenlik agi: normalde duvarlar ve zemin topu iceride tutar
        if (transform.position.y < fallLimitY)
        {
            ResetTo(lastSpawn);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsInLayerMask(collision.collider.gameObject.layer, playerLayer)) return;

        ApplyKick(collision);
    }

    private void ApplyKick(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);

        // Top dairesel: temas noktasindan merkeze giden yon, tam olarak vurusun yonu
        Vector2 kickDir = ((Vector2)transform.position - contact.point).normalized;

        // Kafa gibi ozel bolgeler daha sert vurur; isaretsiz collider'lar 1 katsayili
        KickHitbox hitbox = collision.collider.GetComponent<KickHitbox>();
        float multiplier = hitbox != null ? hitbox.KickMultiplier : 1f;

        Vector2 impulse = kickDir * settings.kickImpulse * multiplier;

        // Kosarken veya ziplarken vurmak, dururken vurmaktan daha sert olsun
        if (collision.rigidbody != null)
        {
            impulse += collision.rigidbody.velocity * settings.playerVelocityTransfer;
        }

        rb.AddForce(impulse, ForceMode2D.Impulse);

        // Yatay vurusta topu dondur: gorsel olarak buyuk fark, maliyeti sifir
        rb.angularVelocity += -kickDir.x * settings.spinPerKick;
    }

    private static bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    public void ResetTo(Vector2 position)
    {
        lastSpawn = position;

        transform.position = position;
        transform.rotation = Quaternion.identity;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    /// <summary>Fizigi tamamen durdurur; gol kutlamasi ve kickoff sirasinda kullanilir.</summary>
    public void SetSimulated(bool value)
    {
        if (!value)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        rb.simulated = value;
    }
}
