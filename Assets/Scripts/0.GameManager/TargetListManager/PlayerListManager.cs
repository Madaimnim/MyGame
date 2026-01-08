using UnityEngine;

[DefaultExecutionOrder(-50)]
public class PlayerListManager : TargetListManager {
    public static PlayerListManager Instance { get; private set; }
    protected override TextAnchor GUIAnchor => TextAnchor.UpperLeft;
    protected override Vector2 GUICornerOffset => new Vector2(10, 10);

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;
    }
}
