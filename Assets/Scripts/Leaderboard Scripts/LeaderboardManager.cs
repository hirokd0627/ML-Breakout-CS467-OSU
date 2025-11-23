using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string name, int score)
    {
        playerName = name;
        this.score = score;
    }
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager instance;

    public List<LeaderboardEntry> topScores = new List<LeaderboardEntry>();

    //persistent copy shared across scenes during runtime
    public static List<LeaderboardEntry> persistentScores = new List<LeaderboardEntry>();

    public static List<LeaderboardEntry> sharedScores = new List<LeaderboardEntry>();

    public GameObject leaderboardPanel;
    public TextMeshProUGUI topScoreText;
    public Button backButton;
    public string mainMenuSceneName = "MenuScene";

    public SoundEffect soundEffect = null; // Used to play sound effect

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (sharedScores != null && sharedScores.Count > 0)
        {
            foreach (var s in sharedScores)
            {
                if (!topScores.Exists(x => x.playerName == s.playerName && x.score == s.score))
                    topScores.Add(new LeaderboardEntry(s.playerName, s.score));
            }
        }

        // ensure sorted & trimmed
        topScores.Sort((a, b) => b.score.CompareTo(a.score));
        if (topScores.Count > 10)
            topScores.RemoveRange(10, topScores.Count - 10);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    void Start()
    {
        if (soundEffect == null)
            throw new ArgumentException("soundEffect");
    }

    // Return whether a score qualifies for top 10
    public bool Qualifies(int score)
    {
        if (topScores.Count < 10)
            return true;

        return score > topScores[topScores.Count - 1].score;
    }

    // Add a new score and update the shared static copy
    public void AddScore(string name, int score)
    {
        // only add the exact pair if not present
        if (!topScores.Exists(x => x.playerName == name && x.score == score))
            topScores.Add(new LeaderboardEntry(name, score));

        topScores.Sort((a, b) => b.score.CompareTo(a.score));
        if (topScores.Count > 10)
            topScores.RemoveRange(10, topScores.Count - 10);

        UpdateTopScoreText();

        if (sharedScores == null)
            sharedScores = new List<LeaderboardEntry>();
        foreach (var e in topScores)
        {
            if (!sharedScores.Exists(x => x.playerName == e.playerName && x.score == e.score))
                sharedScores.Add(new LeaderboardEntry(e.playerName, e.score));
        }
        sharedScores.Sort((a, b) => b.score.CompareTo(a.score));
        if (sharedScores.Count > 10)
            sharedScores.RemoveRange(10, sharedScores.Count - 10);
    }

    public static List<LeaderboardEntry> GetPersistentEntries()
    {
        if (persistentScores == null)
            return new List<LeaderboardEntry>();
        var copy = new List<LeaderboardEntry>(persistentScores);
        copy.Sort((a, b) => b.score.CompareTo(a.score));
        return copy;
    }

    public static List<LeaderboardEntry> GetSharedEntries()
    {
        return new List<LeaderboardEntry>(sharedScores ?? new List<LeaderboardEntry>());
    }

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

    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        // keep game paused while viewing scoreboard
        Time.timeScale = 0f;

        UpdateTopScoreText();
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        // resume game if needed
        Time.timeScale = 1f;
    }

    // resume and go to main menu
    public void OnBackToMainMenu()
    {
        Time.timeScale = 1f; // ensure time resumes before scene load
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            StartCoroutine(soundEffect.PlaySoundAndLoadScene("itemSelect", mainMenuSceneName));
        }
        else
        {
            Debug.LogWarning("[LeaderboardManager] mainMenuSceneName not set.");
        }
    }
}
