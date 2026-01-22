using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameSettingManager : MonoBehaviour {
    public static GameSettingManager Instance { get; private set; }

    public InputSettings InputSettings;
    public PhysicConfig PhysicConfig;
    public SceneConfig SceneConfig;
    public HitShakeConfig HitShakeConfig;
    public ShadowConfig ShadowConfig;

    public SkillCastMode SkillCastMode { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;

        SkillCastMode = InputSettings.SkillCastMode;
    }
}