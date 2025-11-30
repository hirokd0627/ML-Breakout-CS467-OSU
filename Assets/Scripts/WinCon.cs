using UnityEngine;

public class WinCon : MonoBehaviour
{
    public static WinCon instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void HandleGameOver(int humanScore, int cpuScore, int humanBalls, int cpuBalls)
    {
        string winner = DetermineWinner(humanScore, cpuScore, humanBalls, cpuBalls);

        Debug.Log("Winner: " + winner);

        WinUI.instance?.Show(winner, 3f); // show for 3 seconds

        TryShowLeaderboard(humanScore);
    }

    public string DetermineWinner(int humanScore, int cpuScore, int humanBalls, int cpuBalls)
    {
        // Score victory
        if (humanScore >= BrickGeneration.TotalPoint)
            return "Player";
        if (cpuScore >= BrickGeneration.TotalPoint)
            return "CPU";

        // Ball depletion
        if (cpuBalls > 0)
            return "CPU";

        if (cpuBalls == 0)
        {
            if (humanScore > cpuScore)
                return "Player";
            else if (cpuScore > humanScore)
                return "CPU";
            else
                return "Draw";
        }

        // Error handle
        return "Unknown";
    }

    private void TryShowLeaderboard(int humanScore)
    {
        if (!LeaderboardManager.instance.Qualifies(humanScore))
            LeaderboardManager.instance.ShowLeaderboard();

        if (PaddleAgent.IsTraining())
            return;

        UsernameEntryUI.instance?.Show(humanScore);
    }
}
