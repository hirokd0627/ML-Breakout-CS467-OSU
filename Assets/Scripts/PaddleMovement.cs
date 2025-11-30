using System;
using UnityEngine;

public class PaddleMovement : MonoBehaviour
{
    // Serialized values, max and min set in inspector
    public float speed = 18;
    public float maxX = -1.1f;
    public float minX = -7.7f;

    // Whether the paddle moves
    private bool isMovement = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (IsHumanInput() && isMovement)
        {
            // Moves paddle
            transform.position = NextPaddlePosition(
                GetMovementInput(),
                transform,
                minX,
                maxX,
                speed,
                Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Gets input value for paddle movement, positive value is right and negative is left.
    /// </summary>
    public static float GetMovementInput()
    {
        return Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// Returns true if input is for Human, false if input is for CPU.
    /// </summary>
    public static bool IsHumanInput()
    {
        // If shift key is pressed, controls CPU, otherwise controls Human.
        return !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    /// <summary>
    /// Returns next paddle position with input and movement limits.
    /// </summary>
    /// <param name="movement">Movement input, where positive is right and negative is left</param>
    /// <param name="paddle">Transform of paddle</param>
    /// <param name="minX">Left limit for paddle movement</param>
    /// <param name="maxX">Right limit for paddle movement</param>
    /// <param name="speed">Movement speed of paddle</param>
    /// <param name="timeDelta">Time elapsed between frames</param>
    /// <exception cref="ArgumentNullException">If paddle is null</exception>
    public static Vector3 NextPaddlePosition(
        float movement,
        Transform paddle,
        float minX,
        float maxX,
        float speed,
        float timeDelta
    )
    {
        if (paddle == null)
            throw new ArgumentNullException("paddle");

        Vector3 nextPos = paddle.position;

        // Calculate next position if paddle is within movement limits.
        if (
            (movement > 0 && paddle.position.x < maxX) || (movement < 0 && paddle.position.x > minX)
        )
        {
            // NOTE: Depending on input timing, position may exceed minX or maxX.
            //       This will be addressed in another issue.
            nextPos = paddle.position + Vector3.right * movement * speed * timeDelta;

            // Limit movement range
            nextPos.x = Mathf.Min(nextPos.x, maxX);
            nextPos.x = Mathf.Max(nextPos.x, minX);
        }
        return nextPos;
    }

    /// <summary>
    /// Sets to stop moving
    /// </summary>
    public void StopMovement()
    {
        isMovement = false;
    }
}
