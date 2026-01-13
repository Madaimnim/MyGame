using UnityEngine;
using UnityEngine.UI;

public class UI_SubStageButton : MonoBehaviour {
    [Header("StageId")]
    [SerializeField] private int _stageId;   // 101 / 102 / 103（給設計用）

    [Header("鎖頭物件")]
    [SerializeField] private GameObject _lockMask;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        if (button == null) {
            Debug.LogError($"{name} 缺少 Button 元件");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable() {
        Refresh();
    }

    public void Refresh() {
        var stageSystem = GameManager.Instance?.GameStageSystem;
        if (stageSystem == null) return;
        bool unlocked = stageSystem.IsStageUnlocked(_stageId);

        button.interactable = unlocked;
        if (_lockMask != null) _lockMask.SetActive(!unlocked);
    }

    private void OnClick() {
        var stageSystem = GameManager.Instance?.GameStageSystem;
        if (stageSystem == null) return;

        if (!stageSystem.IsStageUnlocked(_stageId)) return;

        stageSystem.RequestEnterStage(_stageId);
    }
}
