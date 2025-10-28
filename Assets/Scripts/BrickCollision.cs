using UnityEngine;

public class BrickCollision : MonoBehaviour
{
    public int points = 1;

    public bool ownerIsCPU;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Add the brick's point value to the total score
            ScoreManager.instance.AddScore(points, ownerIsCPU);

            // Destroy the brick after scoring
            Destroy(gameObject);
        }
    }
}
