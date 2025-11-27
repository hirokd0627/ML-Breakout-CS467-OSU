using System.Collections;
using TMPro;
using UnityEngine;

public class WinUI : MonoBehaviour
{
    public static WinUI instance;

    [Header("Winner UI Elements")]
    [SerializeField]
    private GameObject humanWinsUI;

    [SerializeField]
    private GameObject cpuWinsUI;

    [SerializeField]
    private GameObject drawUI;

    private void Awake()
    {
        instance = this;

        // Hide both at start
        if (humanWinsUI != null)
            humanWinsUI.SetActive(false);
        if (cpuWinsUI != null)
            cpuWinsUI.SetActive(false);
        if (drawUI != null)
            drawUI.SetActive(false);
    }

    /// <summary>
    /// Show winner for a few seconds, then hide
    /// </summary>
    public void Show(string winner, float duration = 3f)
    {
        // Hide both first
        if (drawUI != null)
            drawUI.SetActive(false);
        if (humanWinsUI != null)
            humanWinsUI.SetActive(false);
        if (cpuWinsUI != null)
            cpuWinsUI.SetActive(false);

        GameObject toShow = null;

        if (winner == "Player")
            toShow = humanWinsUI;
        else if (winner == "CPU")
            toShow = cpuWinsUI;
        else if (winner == "Draw")
            toShow = drawUI;

        if (toShow != null)
        {
            toShow.SetActive(true);
            StartCoroutine(HideAfterSeconds(toShow, duration));
        }
    }

    private IEnumerator HideAfterSeconds(GameObject uiObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        uiObject.SetActive(false);
    }
}
