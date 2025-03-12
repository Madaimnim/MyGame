using UnityEngine;

public class UIInputManager : MonoBehaviour
{
    #region 變數定義
    public GameObject inventoryPanel; // 需要切換的 Panel
    #endregion

    #region 更新邏輯
    private void Update() {
        // 只有在 InGame 或 Pause 狀態時，才能使用 I 鍵與 ESC 鍵
        if (GameStateManager.Instance.currentState == GameStateManager.GameState.Preparation)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleInventory();
            }
        }
    }
    #endregion

    #region UI 切換方法
    private void ToggleInventory() {
        bool isActive = inventoryPanel.activeSelf;
        inventoryPanel.SetActive(!isActive);

        // 切換遊戲狀態
        if (isActive)
        {
            //Todo GameStateManager.Instance.SetState(GameStateManager.GameState.InGame);
        }
        else
        {
            //Todo GameStateManager.Instance.SetState(GameStateManager.GameState.Pause);
        }
    }
    #endregion
}