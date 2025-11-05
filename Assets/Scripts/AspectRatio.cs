using UnityEngine;

public class WindowAspectRatio : MonoBehaviour
{
    [SerializeField]
    private float targetAspect = 16f / 9f; //16:9 aspect ratio
    private int lastWidth;
    private int lastHeight;

    void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    void Update()
    {
        // Detect if window size changed
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            // Enforce aspect ratio
            EnforceAspectRatio();
        }
    }

    void EnforceAspectRatio()
    {
        int width = Screen.width;
        int height = Screen.height;

        float currentAspect = (float)width / height;

        if (Mathf.Abs(currentAspect - targetAspect) > 0.01f)
        {
            if (currentAspect > targetAspect)
            {
                // Too wide — adjust width
                width = Mathf.RoundToInt(height * targetAspect);
            }
            else
            {
                // Too tall — adjust height
                height = Mathf.RoundToInt(width / targetAspect);
            }

            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }
    }
}
