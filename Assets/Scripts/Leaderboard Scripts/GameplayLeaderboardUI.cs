using System.Text;
using TMPro;
using UnityEngine;

public class GameplayLeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI listText;
    public int maxEntries = 10;

    void OnEnable() => Refresh();

    public void Refresh()
    {
        if (listText == null)
            return;

        var entries = LeaderboardManager.GetSharedEntries();
        var sb = new StringBuilder();

        if (entries == null || entries.Count == 0)
            sb.AppendLine("No scores yet");
        else
        {
            int count = Mathf.Min(maxEntries, entries.Count);
            for (int i = 0; i < count; i++)
                sb.AppendLine($"{i + 1}. {entries[i].playerName} - {entries[i].score}");
        }

        listText.text = sb.ToString();
    }
}
