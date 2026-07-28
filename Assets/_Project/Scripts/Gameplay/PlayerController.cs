using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] private float speed = 7f;

    [Header("Zıplama")]
    [SerializeField] private float jumpForce = 13f;
    [Tooltip("Zeminden ayrıldıktan sonra hâlâ zıplayabileceğin süre")]
    [SerializeField] private float coyoteTime = 0.1f;
    [Tooltip("Yere değmeden önce basılan zıplamanın hatırlanma süresi")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    [Tooltip("Tuş erken bırakılınca yukarı hız bununla çarpılır")]
    [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;
    [Tooltip("Düşerken yerçekimi bu katsayıyla artar")]
    [SerializeField] private float fallGravityMultiplier = 1.6f;

    [Header("Yer Kontrolü")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float baseGravityScale;

    private float horizontalInput;
    private bool jumpReleased;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool isGrounded;
    private bool inputEnabled = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (!inputEnabled) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump"))
        {
            jumpReleased = true;
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;

        coyoteCounter = isGrounded ? coyoteTime : coyoteCounter - Time.fixedDeltaTime;

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            // İkisini de tüket: aynı basışla ikinci kez zıplanmasın
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        if (jumpReleased)
        {
            // Tuş erken bırakıldıysa yükselişi kes — kısa basış = alçak zıplama
            if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            }

            jumpReleased = false;
        }

        // Düşüş, yükselişten daha hızlı olsun: zıplama "tok" hissettirir
        rb.gravityScale = rb.velocity.y < 0f
            ? baseGravityScale * fallGravityMultiplier
            : baseGravityScale;
    }

    /// <summary>
    /// Kickoff ve gol kutlamasi sirasinda oyuncuyu dondurur. Time.timeScale yerine
    /// bunu kullaniyoruz: sadece oynanis durur, UI ve geri sayim akmaya devam eder.
    /// </summary>
    public void SetInputEnabled(bool value)
    {
        inputEnabled = value;

        if (value) return;

        horizontalInput = 0f;
        jumpBufferCounter = 0f;
        jumpReleased = false;
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public void ResetTo(Vector2 position)
    {
        transform.position = position;

        rb.velocity = Vector2.zero;
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
