using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private PaddleAgent cpuAgent;
    public BallSpeed ballSpeed; // Controls ball speed
    private const float lowerBound = -5f;
    private Rigidbody2D rb;
    private bool gameOver;
    public BallCount ballCount;
    private Vector2 previousVelocity;
    public bool human; // Whether this ball is controlled by Human.

    // minimal bounce angle from horizontal (degrees). Ensures enough vertical velocity.
    private const float minBounceAngleDeg = 30f;

    private Vector3 startPosition;

    public PaddleWidth paddleWidth; // Controls paddle width

    void Awake()
    {
        var go = GameObject.FindWithTag("MLAgent");
        if (go != null)
            cpuAgent = go.GetComponent<PaddleAgent>();
    }

    void Start()
    {
        if (ballSpeed == null)
            throw new ArgumentException("ballSpeed");

        if (paddleWidth == null)
            throw new ArgumentException("paddleWidth");

        startPosition = transform.position;
        LaunchBall();
    }

    void Update()
    {
        float ballPosY = transform.position.y;

        // Relaunch ball or end game
        if (ballPosY < lowerBound && !gameOver)
        {
            if (human)
            {
                gameOver = ballCount.ballCount == 0;
                if (!gameOver)
                {
                    ballCount.decrementBallCount();
                    LaunchBall();
                }
                else
                {
                    gameOver = true;
                    Destroy(gameObject);

                    int finalScore = ScoreManager.instance.humanScore;

                    if (LeaderboardManager.instance.Qualifies(finalScore))
                    {
                        Debug.Log("Score qualifies for leaderboard: " + finalScore);

                        UsernameEntryUI usernameUI = UsernameEntryUI.instance;

                        if (usernameUI != null)
                        {
                            Debug.Log("Found UsernameEntryUI, showing panel.");
                            usernameUI.Show(finalScore);
                        }
                    }
                }
            }
            // TODO: Temporarily spawn infinite balls to avoid crashes until CPU ball management is implemented.
            else //if cpu
            {
                cpuAgent.AddMissPenaltyAndEnd();
                LaunchBall();
            }
        }
    }

    void LaunchBall()
    {
        rb = GetComponent<Rigidbody2D>();

        //reset position
        transform.position = startPosition;

        //reset ball speed
        ballSpeed.ResetSpeed();

        // Launch the ball in a random diagonal direction
        Vector2 direction = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f).normalized;
        rb.linearVelocity = direction * ballSpeed.Speed;
        previousVelocity = rb.linearVelocity;

        //reset paddle width
        paddleWidth.ResetWidth();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle") && !human)
        {
            cpuAgent.AddPaddleHitReward();
        }

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

                rb.linearVelocity = direction * ballSpeed.Speed;
                previousVelocity = rb.linearVelocity;
            }
            else // left or right side
            {
                // Since ball is simply lost, so it bounces at same angle as angle of incidence.
                rb.linearVelocity = new Vector2(-1.0f * previousVelocity.x, previousVelocity.y);
                previousVelocity = rb.linearVelocity;
            }
        }
        else
        {
            ClampBounceAngle();
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

    private void ClampBounceAngle()
    {
        /*
        ensure the velocity has at least minBounceAngleDeg from horizontal
        */

        // angle in degrees between velocity and +X axis
        float angleDeg = Mathf.Abs(
            Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg
        );
        // convert this to the minimum angle from horizontal
        float angleFromHorizontal = Mathf.Min(angleDeg, 180f - angleDeg);

        //now check to see if its below the threshold (too steep)
        if (angleFromHorizontal < minBounceAngleDeg)
        {
            // decide direction signs (preserve general left/right and up/down)
            float signX = Mathf.Sign(rb.linearVelocity.x);
            float signY = Mathf.Sign(rb.linearVelocity.y);

            // fallback when vertical sign is exactly zero, otherwise newVy will be zero still
            if (Mathf.Approximately(signY, 0f))
            {
                signY = Mathf.Sign(previousVelocity.y != 0f ? previousVelocity.y : 1f);
            }

            float angRad = Mathf.Deg2Rad * minBounceAngleDeg;
            float newVx = signX * Mathf.Cos(angRad) * rb.linearVelocity.magnitude;
            float newVy = signY * Mathf.Sin(angRad) * rb.linearVelocity.magnitude;

            rb.linearVelocity = new Vector2(newVx, newVy);
            previousVelocity = rb.linearVelocity;
        }
    }

    /// <summary>
    /// Sets ball speed, preserving its direction.
    /// </summary>
    /// <param name="speed">ball speed</param>
    public void SetSpeed(float speed)
    {
        Vector2 direction = rb.linearVelocity.normalized; // keep direction
        rb.linearVelocity = direction * speed;
        previousVelocity = rb.linearVelocity;
    }
}
