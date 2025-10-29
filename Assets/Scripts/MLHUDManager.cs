using TMPro;
using UnityEngine;

public class MLHUDManager : MonoBehaviour
{
    public TMP_Text episodeText;
    public TMP_Text totalRewardsText;
    public TMP_Text stepText;
    public ScoreManager scoreManager;
    public PaddleAgent paddleAgent;

    void Start()
    {
        if (scoreManager == null)
            scoreManager = ScoreManager.instance;

        if (paddleAgent == null)
        {
            var go = GameObject.FindWithTag("MLAgent");
            if (go != null)
                paddleAgent = go.GetComponent<PaddleAgent>();
        }
    }

    void Update()
    {
        if (episodeText != null)
            episodeText.text =
                "Episode: " + (paddleAgent != null ? paddleAgent.episodeCount.ToString() : "0");

        if (totalRewardsText != null)
            totalRewardsText.text =
                "Total Rewards: "
                + (paddleAgent != null ? paddleAgent.GetCumulativeReward().ToString("F2") : "0");

        if (stepText != null)
            stepText.text =
                "Step: " + (paddleAgent != null ? paddleAgent.stepCount.ToString() : "0");
    }
}
