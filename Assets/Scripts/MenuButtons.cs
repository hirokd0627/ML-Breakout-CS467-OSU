using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [Header("Scene Names")]
    public string mainMenuSceneName = "MenuScene"; // set this in inspector

    [Header("Menu References")]
    public GameObject mainMenu; // assign Main Menu GameObject in Inspector
    public GameObject difficultyMenu; // assign Difficulty Menu GameObject in Inspector
    public GameObject leaderboardMenu; // assign leaderboard game object
    public int difficulty; // Storage for game dofficulty selection. 1 = easy, 2 = medium, 3 = hard

    public void StartGame()
    {
        // Deactivate the main menu
        if (mainMenu != null)
        {
            mainMenu.SetActive(false);
        }

        // Activate difficulty menu
        if (difficultyMenu != null)
        {
            difficultyMenu.SetActive(true);
        }
    }

    public void OpenLeaderboard()
    {
        if (mainMenu != null)
            mainMenu.SetActive(false);
        if (difficultyMenu != null)
            difficultyMenu.SetActive(false);
        if (leaderboardMenu != null)
            leaderboardMenu.SetActive(true);
        if (LeaderboardManager.instance != null)
            LeaderboardManager.instance.ShowLeaderboard();
    }

    public void GoBack()
    {
        // ensure game is unpaused
        Time.timeScale = 1f;
        if (leaderboardMenu != null && leaderboardMenu.activeSelf && mainMenu != null)
        {
            leaderboardMenu.SetActive(false);
            mainMenu.SetActive(true);
            return;
        }

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }
        if (mainMenu != null)
            mainMenu.SetActive(true);
    }

    public void StartEasy()
    {
        difficulty = 1; // setting before scene load
        SceneManager.LoadScene("GameplayScene");
    }

    public void StartMedium()
    {
        difficulty = 2;
        SceneManager.LoadScene("GameplayScene");
    }

    public void StartHard()
    {
        difficulty = 3;
        SceneManager.LoadScene("GameplayScene");
    }
}
