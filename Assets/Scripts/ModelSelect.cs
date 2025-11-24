using Unity.MLAgents.Policies; // BehaviorParameters & BehaviorType
using UnityEngine;

[DisallowMultipleComponent]
public class PaddleAgentModelSelector : MonoBehaviour
{
    [Header("Game Difficulty")]
    [Range(1, 3)]
    public int difficulty; // 1 = easy, 2 = medium, 3 = hard

    [Header("Drag & Drop Models")]
    public Unity.InferenceEngine.ModelAsset LowLevel;
    public Unity.InferenceEngine.ModelAsset MediumLevel;
    public Unity.InferenceEngine.ModelAsset HardLevel;

    private BehaviorParameters behaviorParameters;

    void Awake()
    {
        behaviorParameters = GetComponent<BehaviorParameters>();
        if (behaviorParameters == null)
        {
            Debug.LogError("BehaviorParameters component missing!");
            return;
        }

        // Read difficulty from GameSettings
        difficulty = GameSettings.SelectedDifficulty;

        ApplyDifficultyModel();
    }

    void Start()
    {
        // Set inference only if not training
        if (!PaddleAgent.IsTraining())
        {
            behaviorParameters.BehaviorType = BehaviorType.InferenceOnly;
        }
    }

    // Apply difficulty model based on public int difficulty
    public void ApplyDifficultyModel()
    {
        if (behaviorParameters == null)
            return;

        switch (difficulty)
        {
            case 1:
                behaviorParameters.Model = LowLevel;
                break;
            case 2:
                behaviorParameters.Model = MediumLevel;
                break;
            case 3:
                behaviorParameters.Model = HardLevel;
                break;
        }

        // Force agent to use inference
        behaviorParameters.BehaviorType = BehaviorType.InferenceOnly;

        Debug.Log($"Applied difficulty {difficulty} model: {behaviorParameters.Model.name}");
    }

    // Change difficulty at runtime
    public void SetDifficulty(int newDifficulty)
    {
        difficulty = Mathf.Clamp(newDifficulty, 1, 3);
        ApplyDifficultyModel();
    }
}
