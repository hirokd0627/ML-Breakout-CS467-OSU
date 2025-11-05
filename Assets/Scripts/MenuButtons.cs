using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
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
    }

    public void GoBack()
    {
        // Activate the main menu
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }

        //Deactivate leaderboard menu
        if (leaderboardMenu != null)
        {
            leaderboardMenu.SetActive(false);
        }

        // Deactivate difficulty menu
        if (difficultyMenu != null)
        {
            difficultyMenu.SetActive(false);
        }
    }

    public void StartEasy()
    {
        // Load the GameplayScene, easy difficulty
        SceneManager.LoadScene("GameplayScene");
        difficulty = 1;
    }

    public void StartMedium()
    {
        // Load the GameplayScene, medium difficulty
        SceneManager.LoadScene("GameplayScene");
        difficulty = 2;
    }

    public void StartHard()
    {
        // Load the GameplayScene, hard difficulty
        SceneManager.LoadScene("GameplayScene");
        difficulty = 3;
    }
}
