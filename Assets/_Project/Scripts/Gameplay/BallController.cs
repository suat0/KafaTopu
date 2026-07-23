using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.y < -6f)
        {
            Debug.Log("Ball fell below the screen. Resetting position.");
            rb.velocity = Vector2.zero; // Reset velocity to stop the ball from moving
            transform.position = new Vector3(0, 0, 0);
           
        }
    }
}