using UnityEngine;

public class BrickCollision : MonoBehaviour
{
    public int points = 1;

    public bool ownerIsCPU;

    private static PaddleAgent cpuAgent;

    void Awake()
    {
        if (cpuAgent == null)
        {
            var go = GameObject.FindWithTag("MLAgent");
            cpuAgent = go.GetComponent<PaddleAgent>();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var ball = collision.gameObject.GetComponent<Ball>(); //need this to get to the human/cpu attributes
        if (ball == null)
            return; //dont proceed if not a ball collision

        // Add the brick's point value to the total score
        ScoreManager.instance.AddScore(points, ownerIsCPU);

        if (!ball.human) //add small reward for the cpu brick hits
        {
            cpuAgent.AddBrickReward();
        }

        // Destroy the brick after scoring
        Destroy(gameObject);

        if (!PaddleAgent.IsTraining())
        {
            // When all the bricks are broken, ends the game.
            int finalScore = ScoreManager.instance.humanScore;
            int cpuScore = ScoreManager.instance.cpuScore;
            if (
                (finalScore >= BrickGeneration.TotalPoint)
                || (cpuScore >= BrickGeneration.TotalPoint)
            )
            {
                GameOverManager.instance.EndGame();
            }
        }

        if (!ball.human)
        {
            cpuAgent.DecrementBrickCount();
        }
    }
}
