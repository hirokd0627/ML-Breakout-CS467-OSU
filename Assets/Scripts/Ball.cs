using UnityEngine;

public class Ball : MonoBehaviour
{
    private const float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Launch the ball in a random diagonal direction
        Vector2 direction = new Vector2(Random.Range(-1f, 1f), 1f).normalized;
        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle")) // collided with paddle
        {
            // Change ball bounce angle
            // Divide paddle into four sections and change angle starting from left to right.
            // most left: -60, left: -30, right: 30, most right: 60 (angles are in degrees relative to Y+).
            float paddlePosX = collision.transform.position.x;
            float paddleLen = collision.collider.bounds.size.x;
            float ballPosX = transform.position.x;
            float posX = (ballPosX - paddlePosX) / paddleLen; // Normalized position [-0.5, 0.5]

            float deg;
            if (posX < -0.25f) // most left
            {
                deg = 150.0f; // -60
            }
            else if (posX < 0.0) // left
            {
                deg = 120.0f; // -30
            }
            else if (posX < 0.25f) // right
            {
                deg = 60.0f; // 30
            }
            else // most right
            {
                deg = 30.0f; // 60
            }

            float rad = Mathf.Deg2Rad * deg;
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            rb.linearVelocity = direction * speed;
        }
    }
}
