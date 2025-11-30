using TMPro;
using UnityEngine;

// Handles game termination
public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;
    public Ball humanBall;
    public Ball cpuBall;
    public PaddleAgent cpuAgent;
    public PaddleMovement humanPaddle;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void EndGame()
    {
        int finalScore = ScoreManager.instance.humanScore;
        int cpuScore = ScoreManager.instance.cpuScore;

        int humanBallCount = humanBall.ballCount.ballCount;
        int cpuBallCount = cpuBall.ballCount.ballCount;

        if (humanBall)
            humanBall.Destroy();
        if (cpuBall)
            cpuBall.Destroy();

        humanPaddle.StopMovement();
        cpuAgent.StopMovement();

        WinCon.instance.HandleGameOver(finalScore, cpuScore, humanBallCount, cpuBallCount);
    }
}
