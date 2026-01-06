using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameSettingManager : MonoBehaviour {
    public static GameSettingManager Instance { get; private set; }

    public InputSettings InputSettings;

    [Header("Input Settings")]
    public SkillCastMode SkillCastMode;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SkillCastMode = InputSettings.SkillCastMode;
    }
}