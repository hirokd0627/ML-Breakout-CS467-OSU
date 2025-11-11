using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UsernameEntryUI : MonoBehaviour
{
    public static UsernameEntryUI instance;

    public TMP_InputField inputField;
    public Button submitButton;
    public GameObject panel;

    private int pendingScore;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        if (panel != null)
            panel.SetActive(false);
    }

    // Call this when player qualifies
    public void Show(int score)
    {
        pendingScore = score;
        panel.SetActive(true);
        Time.timeScale = 0f;
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    void OnSubmit()
    {
        string playerName = inputField.text;
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player";
        if (LeaderboardManager.instance != null)
            LeaderboardManager.instance.AddScore(playerName, pendingScore);
        else
            Debug.LogWarning("[UsernameEntryUI] LeaderboardManager.instance is null"); //error check

        panel.SetActive(false);
        if (LeaderboardManager.instance != null)
            LeaderboardManager.instance.ShowLeaderboard();
    }
}
