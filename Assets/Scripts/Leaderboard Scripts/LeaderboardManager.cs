using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string name, int score)
    {
        this.playerName = name;
        this.score = score;
    }
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    public List<LeaderboardEntry> topScores = new List<LeaderboardEntry>();

    public GameObject leaderboardPanel;
    public TextMeshProUGUI topScoreText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    // Show/Hide helpers
    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        UpdateTopScoreText();
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    // Return whether a score qualifies for top 10
    public bool Qualifies(int score)
    {
        if (topScores.Count < 10)
            return true;

        return score > topScores[topScores.Count - 1].score;
    }

    // Add a new score and sort
    public void AddScore(string name, int score)
    {
        topScores.Add(new LeaderboardEntry(name, score));
        topScores.Sort((a, b) => b.score.CompareTo(a.score));
        if (topScores.Count > 10)
            topScores.RemoveAt(topScores.Count - 1);

        UpdateTopScoreText();
    }

    // Update the single "highest score" text field
    public void UpdateTopScoreText()
    {
        if (topScoreText == null)
            return;

        if (topScores.Count == 0)
        {
            topScoreText.text = "No scores yet";
            return;
        }

        var top = topScores[0];
        topScoreText.text = $"{top.playerName} - {top.score}";
    }
}
