using System;
using System.Collections;
using Unity.VisualScripting;
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

    [Header("Sound effect")]
    public SoundEffect soundEffect = null; // Used to play sound effect

    void Awake()
    {
        // If training, skip menu selection and start game.
        if (PaddleAgent.IsTraining())
        {
            StartEasy();
        }
    }

    void Start()
    {
        if (soundEffect == null)
            throw new ArgumentException("soundEffect");
    }

    public void StartGame()
    {
        soundEffect.PlaySound("itemSelect");

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
        soundEffect.PlaySound("itemSelect");

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
            soundEffect.PlaySound("itemSelect");

            leaderboardMenu.SetActive(false);
            mainMenu.SetActive(true);
            return;
        }

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            StartCoroutine(soundEffect.PlaySoundAndLoadScene("itemSelect", mainMenuSceneName));
            return;
        }
        if (mainMenu != null)
        {
            soundEffect.PlaySound("itemSelect");
            mainMenu.SetActive(true);
        }
    }

    public void StartEasy()
    {
        difficulty = 1; // setting before scene load
        StartCoroutine(soundEffect.PlaySoundAndLoadScene("itemSelect", "GameplayScene"));
    }

    public void StartMedium()
    {
        difficulty = 2;
        StartCoroutine(soundEffect.PlaySoundAndLoadScene("itemSelect", "GameplayScene"));
    }

    public void StartHard()
    {
        difficulty = 3;
        StartCoroutine(soundEffect.PlaySoundAndLoadScene("itemSelect", "GameplayScene"));
    }
}
