using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int humanScore = 0;
    public int cpuScore = 0;

    public TMP_Text humanScoreText;
    public TMP_Text cpuScoreText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int points, bool ownerIsCPU)
    {
        if (ownerIsCPU)
        {
            cpuScore += points;
        }
        else
        {
            humanScore += points;
        }
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (humanScoreText != null)
            humanScoreText.text = "Score: " + humanScore;
        if (cpuScoreText != null)
            cpuScoreText.text = "Score: " + cpuScore;
    }
}
