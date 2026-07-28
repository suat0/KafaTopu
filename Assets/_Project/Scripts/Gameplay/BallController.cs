using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [SerializeField] private BallSettings settings;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private ScoreData scoreData;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private float fallLimitY = -6f;

    private Rigidbody2D rb;
    private bool isGameOver;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isGameOver && transform.position.y < fallLimitY)
        {
            isGameOver = true;

            if (gameOverUI != null)
            {
                gameOverUI.Show();
            }
        }
    }

    void FixedUpdate()
    {
        // Sinirsiz hizlanan top collider'lari atlar ve oynanamaz hale gelir
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, settings.maxSpeed);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -settings.maxSpin, settings.maxSpin);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsInLayerMask(collision.collider.gameObject.layer, playerLayer)) return;

        scoreData.AddPoint(1);
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

    public void ResetBall()
    {
        transform.position = Vector3.zero;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        isGameOver = false;
    }
}
