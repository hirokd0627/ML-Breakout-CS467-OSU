using UnityEngine;

public class BallCPU : MonoBehaviour
{
    private const float speed = 5f;
    private const float lowerBound = -6f;
    private Rigidbody2D rb;
    private bool gameOver;
    public BallCount ballCount;
    private Vector2 previousVelocity;

    void Start()
    {
        LaunchBall();
    }

    void Update()
    {
        float ballPosY = transform.position.y;

        // Relaunch ball or end game
        if (ballPosY < lowerBound && !gameOver)
        {
            gameOver = ballCount.ballCount == 0;
            if (!gameOver)
            {
                ballCount.decrementBallCount();
                LaunchBall();
            }
        }
    }

    void LaunchBall()
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
            if (HitPaddleTopSide(collision)) // top side
            {
                // Change ball bounce angle
                // Divide paddle into four sections and change angle starting from left to right.
                // most left: -60, left: -30, right: 30, most right: 60 (angles are in degrees relative to Y+).
                //(Actual degrees are relative to X+, commented are relative to Y+ for clarity)
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
            else // left or right side
            {
                // Since ball is simply lost, so it bounces at same angle as angle of incidence.
                rb.linearVelocity = new Vector2(-1.0f * previousVelocity.x, previousVelocity.y);
            }
        }
    }

    void FixedUpdate()
    {
        // Ball's physic bounce is set to 0 to prevent jitter when hitting paddle, so Y velocity is always 0.
        // in OnCollisionEnter2D. Stores velocity before collision to control side bounces.
        previousVelocity = rb.linearVelocity;
    }

    bool HitPaddleTopSide(Collision2D collision)
    {
        if (collision.contactCount == 0)
        {
            return false;
        }

        // There should be only one contact point.
        ContactPoint2D contact = collision.GetContact(0);

        // normal vector angle
        Vector2 normal = collision.contacts[0].normal;
        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg; // [-180, 180]

        // If ball hits the top side of paddle, normal vector points upward (angle = 90).
        // If ball hits paddle corner, angle is not 90, for simplicity, use 45 degree as boundary.
        return angle >= 45.0f && angle <= 135.0f;
    }
}
