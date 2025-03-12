using UnityEngine;

public class ButtonString : MonoBehaviour
{
    public string changeGameStateName; // 在 Inspector 設定此值

    public void ChangeGameStateMethod() {
        if (System.Enum.TryParse(changeGameStateName, out GameStateManager.GameState newState))
        {
            GameStateManager.Instance.SetState(newState);
        }
        else
        {
            Debug.LogError($"❌ 無法解析 GameState: {changeGameStateName}，請確保輸入正確！");
        }
    }
}
