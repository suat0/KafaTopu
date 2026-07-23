using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private GameOverUI gameOverUI;
    private bool isGameOver = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isGameOver && transform.position.y < -6f)
        {
            gameOverUI.Show();
            isGameOver = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            scoreData.AddPoint(1);
            Debug.Log("Score: " + scoreData.GetScore());
        }
    }

    public void ResetBall()
    {
        transform.position = Vector3.zero;
        rb.velocity = Vector2.zero;
        isGameOver = false;
    }

}