using System;
using UnityEngine;

/// <summary>
/// Updates the paddle width.
/// Paddle width is halved when the ball collides with the top wall.
/// Attach this script to ball object.
/// </summary>
public class PaddleWidth : MonoBehaviour
{
    /// <summary>
    /// Whether it is controlled by human.
    /// </summary>
    [SerializeField]
    private bool human;

    /// <summary>
    /// Paddle game object
    /// </summary>
    [SerializeField]
    private GameObject paddle;

    /// <summary>
    /// PaddleMovement Script attached to human paddle
    /// Assign if this ball is controlled by human
    /// </summary>
    [SerializeField]
    private PaddleMovement paddleMovement = null;

    /// <summary>
    /// PaddleAgent related to CPU paddle
    /// Assign if this ball is controlled by CPU
    /// </summary>
    [SerializeField]
    private PaddleAgent paddleAgent = null;

    /// <summary>
    /// Whether paddle has been halved.
    /// </summary>
    private bool isHalved = false;

    /// <summary>
    /// Initial paddle transform.localScale.x
    /// </summary>
    private float initialPaddleWidth;

    /// <summary>
    /// Initial paddle movement range minimum X.
    /// </summary>
    private float initialMovementRangeMinX;

    /// <summary>
    /// Initial paddle movement range maximum X.
    /// </summary>
    private float initialMovementRangeMaxX;

    /// <summary>
    /// Awake is called when loading an instance of a script component.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    void Awake()
    {
        if (paddle == null)
            throw new ArgumentNullException("paddle");

        if (human)
        {
            if (paddleMovement == null)
                throw new ArgumentNullException("paddleMovement");

            initialMovementRangeMinX = paddleMovement.minX;
            initialMovementRangeMaxX = paddleMovement.maxX;
        }
        else
        {
            if (paddleAgent == null)
                throw new ArgumentNullException("paddleAgent");

            initialMovementRangeMinX = paddleAgent.paddleMoveMinX;
            initialMovementRangeMaxX = paddleAgent.paddleMoveMaxX;
        }

        initialPaddleWidth = paddle.transform.localScale.x;
    }

    /// <summary>
    /// Resets paddle width.
    /// </summary>
    public void ResetWidth()
    {
        SetPaddleWidth(false);
    }

    /// <summary>
    /// Called when object's collider makes contact with this collider.
    /// </summary>
    /// <param name="collision">Collision2D of contacted collider</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TopWall")) // collide with top wall
        {
            SetPaddleWidth(true);
        }
    }

    /// <summary>
    /// Sets paddle width and extends movement range.
    /// </summary>
    void SetPaddleWidth(bool half)
    {
        if (isHalved && half)
        {
            // Already paddle is halved
            return;
        }

        isHalved = half;

        // Sets paddle width
        Vector3 nextScale = paddle.transform.localScale;
        float newWidth = initialPaddleWidth;
        if (half)
            newWidth /= 2.0f;

        nextScale.x = newWidth;
        paddle.transform.localScale = nextScale;

        // Expand movement range to compensate for reduced paddle width
        float minX = initialMovementRangeMinX;
        float maxX = initialMovementRangeMaxX;
        if (half)
        {
            minX -= newWidth / 2.0f;
            maxX += newWidth / 2.0f;
        }
        if (human)
        {
            paddleMovement.minX = minX;
            paddleMovement.maxX = maxX;
        }
        else
        {
            paddleAgent.paddleMoveMinX = minX;
            paddleAgent.paddleMoveMaxX = maxX;
        }

        // Keep paddle within movement range to avoid clipping into wall when resetting to its original width.
        if (!half)
        {
            Vector3 pos = paddle.transform.position;
            if (human)
            {
                pos.x = Mathf.Max(pos.x, paddleMovement.minX);
                pos.x = Mathf.Min(pos.x, paddleMovement.maxX);
            }
            else
            {
                pos.x = Mathf.Max(pos.x, paddleAgent.paddleMoveMinX);
                pos.x = Mathf.Min(pos.x, paddleAgent.paddleMoveMaxX);
            }
            paddle.transform.position = pos;
        }
    }
}
