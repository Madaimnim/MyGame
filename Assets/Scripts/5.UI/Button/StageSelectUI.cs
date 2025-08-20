using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public Button[] stageButtons;          // Inspector 拖入 Stage1、Stage2、Stage3 按鈕
    public string[] stageSceneNames;       // Inspector 填 Stage1Scene、Stage2Scene、Stage3Scene

    private void Start() {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int index = i; // 避免閉包問題
            stageButtons[i].onClick.AddListener(() => {
                string targetScene = stageSceneNames[index];
                GameStateManager.Instance.SetState(GameStateManager.GameState.Battle, targetScene);
            });
        }
    }
}
