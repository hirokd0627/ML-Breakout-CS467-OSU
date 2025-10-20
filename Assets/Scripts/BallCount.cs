using TMPro;
using UnityEngine;

public class BallCount : MonoBehaviour
{
    private int _ballCount;
    public int ballCount
    {
        get { return _ballCount; }
        set { _ballCount = value; }
    }
    public TMP_Text remainingBallsText;

    void Start()
    {
        _ballCount = 4;
    }

    void Update()
    {
        remainingBallsText.text = "Remaining Balls: " + ballCount;
    }

    public void decrementBallCount()
    {
        _ballCount -= 1;
    }
}
