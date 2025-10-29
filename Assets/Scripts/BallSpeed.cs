using System;
using UnityEngine;

/// <summary>
/// Updates the ball speed.
/// Speed is increased when the ball's bounce count exceeds a specific count,
/// or when it collides with a red or orange brick.
/// Attach this script to ball object.
/// </summary>
public class BallSpeed : MonoBehaviour
{
    /// <summary>
    /// Ball script attached to the same ball object
    /// </summary>
    [SerializeField]
    private Ball ball;

    [Header("Bounce Threshold")]
    /// <summary>
    /// Bounce count threshold for low speed
    /// </summary>
    [SerializeField]
    private int bounceThresholdLow = 4;

    /// <summary>
    /// Bounce count threshold for medium speed
    /// </summary>
    [SerializeField]
    private int bounceThresholdMedium = 12;

    [Header("Ball Speed")]
    /// <summary>
    /// Initial ball speed
    /// </summary>
    [SerializeField]
    private float ballSpeedInitial = 5.0f;

    /// <summary>
    /// Low ball speed; Used after ball bounces off paddle 4 times
    /// </summary>
    [SerializeField]
    private float ballSpeedLow = 6.5f;

    /// <summary>
    /// Medium ball speed; Used after ball bounces off paddle 4 times
    /// </summary>
    [SerializeField]
    private float ballSpeedMedium = 8.0f;

    /// <summary>
    /// High ball speed; Used after ball bounces off paddle 4 times
    /// </summary>
    [SerializeField]
    private float ballSpeedHigh = 10.0f;

    [Header("Debug")]
    /// <summary>
    /// Enable to output debug log to console.
    /// </summary>
    [SerializeField]
    private bool logObservations = false;

    /// <summary>
    /// Ball speeds: 0:initial, 1:low, 2:medium, 3:high
    /// </summary>
    private float[] Speeds;

    /// <summary>
    /// Current index of Speeds
    /// </summary>
    private int speedIndex = 0;

    /// <summary>
    /// Current bounce count
    /// </summary>
    private int bounceCount = 0;

    /// <summary>
    /// Gets current ball speed.
    /// </summary>
    public float Speed
    {
        get
        {
            if (speedIndex < 0 || speedIndex > 3)
                throw new ArgumentOutOfRangeException(
                    nameof(speedIndex),
                    speedIndex,
                    "invalid speedIndex"
                );

            return Speeds[speedIndex];
        }
    }

    /// <summary>
    /// Awake is called when loading an instance of a script component.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    void Awake()
    {
        if (ball == null)
            throw new ArgumentNullException("ball");

        // Initialize Speeds for simplified speed management
        Speeds = new float[] { ballSpeedInitial, ballSpeedLow, ballSpeedMedium, ballSpeedHigh };

        // Set initial speed
        speedIndex = 0;
    }

    /// <summary>
    /// Resets ball speed.
    /// </summary>
    public void ResetSpeed()
    {
        bounceCount = 0;
        speedIndex = 0;
    }

    /// <summary>
    /// Called when object's collider makes contact with this collider.
    /// </summary>
    /// <param name="collision">Collision2D of contacted collider</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick")) // collide with brick
        {
            CollisionWithBrick(collision);
        }
        else if (collision.gameObject.CompareTag("Paddle")) // collide with paddle
        {
            IncrementBounceCount();
        }
    }

    /// <summary>
    /// Increments the total ball's bounce count.
    /// If the bounce count reaches thresholds, changes ball speed.
    /// </summary>
    private void IncrementBounceCount()
    {
        bounceCount += 1;

        DebugLog(
            string.Format(
                "BallSpeed: collided with paddle human={0} bounce={1} speed={2}",
                ball.human,
                bounceCount,
                Speed
            )
        );

        if (speedIndex == (Speeds.Length - 1)) // Already at HIGH
            return;

        if (bounceCount < bounceThresholdLow) // Below LOW
            return;

        // Change speed based on thresholds
        int newSpeedIndex = (bounceCount >= bounceThresholdMedium) ? 2 : 1;
        if (newSpeedIndex == speedIndex) // current speed based on the bounce count
            return;
        speedIndex = newSpeedIndex;
        ball.SetSpeed(Speed);
        DebugLog(
            string.Format(
                "BallSpeed: change speed human={0} bounce={1} speed={2}",
                ball.human,
                bounceCount,
                Speed
            )
        );
    }

    /// <summary>
    /// Handles logic when ball collides with brick.
    /// </summary>
    /// <param name="collision">Collision2D of contacted brick's collider</param>
    /// <exception cref="ArgumentNullException"></exception>
    void CollisionWithBrick(Collision2D collision)
    {
        if (speedIndex == (Speeds.Length - 1)) // Already at HIGH
            return;

        // Get brick color
        SpriteRenderer spriteRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            throw new ArgumentNullException("spriteRenderer");

        ChangeBallSpeedWithBrickColor(spriteRenderer.color);
    }

    /// <summary>
    /// Changes ball speed based on brick color.
    /// </summary>
    /// <param name="brickColor">color of brick collided with ball</param>
    void ChangeBallSpeedWithBrickColor(UnityEngine.Color brickColor)
    {
        // Set HIGH speed if brick's color is red or orange
        if (brickColor == UnityEngine.Color.red || brickColor == BrickGeneration.BrickColorOrange)
        {
            speedIndex = Speeds.Length - 1;
            ball.SetSpeed(Speed);

            DebugLog(
                string.Format(
                    "BallSpeed: collided with a red or orange brick, change speed human={0} bounce={1} speed={2}",
                    ball.human,
                    bounceCount,
                    Speed
                )
            );
        }
    }

    /// <summary>
    /// Outputs debug log.
    /// </summary>
    /// <param name="message">log message</param>
    void DebugLog(string message)
    {
        if (logObservations)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
