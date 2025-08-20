using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public Button[] stageButtons;          // Inspector ��J Stage1�BStage2�BStage3 ���s
    public string[] stageSceneNames;       // Inspector �� Stage1Scene�BStage2Scene�BStage3Scene

    private void Start() {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int index = i; // �קK���]���D
            stageButtons[i].onClick.AddListener(() => {
                string targetScene = stageSceneNames[index];
                GameStateManager.Instance.SetState(GameStateManager.GameState.Battle, targetScene);
            });
        }
    }
}
