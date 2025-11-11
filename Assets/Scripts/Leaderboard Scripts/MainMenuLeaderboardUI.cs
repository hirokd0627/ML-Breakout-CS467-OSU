using TMPro;
using UnityEngine;

public class MainMenuLeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI listText;
    public int maxEntriesToShow = 10;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (listText == null)
            return;

        // use the shared in-memory list from LeaderboardManager
        var entries = LeaderboardManager.GetSharedEntries();

        var sb = new System.Text.StringBuilder();
        if (entries == null || entries.Count == 0)
        {
            sb.AppendLine("No scores yet");
        }
        else
        {
            int count = Mathf.Min(maxEntriesToShow, entries.Count);
            for (int i = 0; i < count; i++)
            {
                var e = entries[i];
                sb.AppendLine($"{i + 1}. {e.playerName} - {e.score}");
            }
        }

        listText.text = sb.ToString();
    }
}
