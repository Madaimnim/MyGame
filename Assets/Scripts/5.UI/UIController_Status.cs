using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UIController_Status : MonoBehaviour
{
    public Button ChangNextPlayerDisplayerButton;
    public Button ChangLastPlayerDisplayerButton;
    public TextMeshProUGUI[] statusTextsArray; // 角色基本狀態顯示（前 6 個）
    private PlayerStatsRuntime currentPlayer;


    private void OnEnable() {
        EventBus.Listen<UICurrentPlayerChangEvent>(OnUICurrentPlayerChanged);
        StartCoroutine(WaitAndInit());
    }

    private void OnDisable() {
        EventBus.StopListen<UICurrentPlayerChangEvent>(OnUICurrentPlayerChanged);
        ChangNextPlayerDisplayerButton.onClick.RemoveListener(ChangNextPlayerDisplay);
        ChangLastPlayerDisplayerButton.onClick.RemoveListener(ChangLastPlayerDisplay);
    }


    private IEnumerator WaitAndInit() {
        yield return new WaitUntil(() => UIManager.Instance != null && PlayerStateManager.Instance != null);
        yield return new WaitUntil(() => UIManager.Instance.currentPlayerId != -1);

        ChangNextPlayerDisplayerButton.onClick.AddListener(ChangNextPlayerDisplay);
        ChangLastPlayerDisplayerButton.onClick.AddListener(ChangLastPlayerDisplay);

        if (!PlayerStateManager.Instance.TryGetState(UIManager.Instance.currentPlayerId, out currentPlayer))
        {
            Debug.LogError($"❌ WaitAndInit: 找不到 ID={UIManager.Instance.currentPlayerId} 的玩家狀態");
            yield break; // 中止，避免後面 Null
        }

        SetActiveUIPlayer(UIManager.Instance.currentPlayerId);
        RefreshPlayerStatusText();
    }


    private void OnUICurrentPlayerChanged(UICurrentPlayerChangEvent eventData) {
        currentPlayer = eventData.currentPlayer;
        SetActiveUIPlayer(currentPlayer.Id);
        RefreshPlayerStatusText();
    }

    #region RefreshPlayerStatusText()
    //更新狀態文字，執行RenderTexture撥放角色預覽動畫
    private void RefreshPlayerStatusText() {
        if (currentPlayer == null)
        {
            Debug.LogError("❌ RefreshPlayerStatusText: currentPlayer 為 null");
            return;
        }
        if (statusTextsArray == null || statusTextsArray.Length < 6)
        {
            Debug.LogError("❌ RefreshPlayerStatusText: statusTextsArray 沒有綁定或數量不足");
            return;
        }

        statusTextsArray[0].text = $"等級: {currentPlayer.Level}";
        statusTextsArray[1].text = $"HP: {currentPlayer.MaxHp}/{currentPlayer.MaxHp}";
        statusTextsArray[2].text = $"名稱: {currentPlayer.Name}";
        statusTextsArray[3].text = $"攻擊力: {currentPlayer.AttackPower}";
        statusTextsArray[4].text = $"經驗值: {currentPlayer.CurrentExp}/{currentPlayer.CurrentExp}";
        statusTextsArray[5].text = $"速度: {currentPlayer.MoveSpeed}";
    }
    #endregion

    #region SetActiveUIPlayer(int playerID)
    //每切換當前UI角色執行，將activePlayerUIDtny中的角色物件Active
    public void SetActiveUIPlayer(int playerID) {
        foreach (var kvp in UIManager.Instance.activeUIPlayersDtny) // 遍歷所有角色 UI
        {
            bool isActive = (kvp.Key == playerID); // 只有當前角色 UI 設為 true
            kvp.Value.SetActive(isActive);
            Animator animator = kvp.Value.GetComponent<Animator>();
            if (isActive) // 如果是當前選擇的角色，播放動畫
                animator.Play(Animator.StringToHash($"Skill1")); // 播放指定動畫
        }
    }
    #endregion

    void ChangNextPlayerDisplay() {
        UIManager.Instance.ChangCurrentPlayerID(1);
    }

    void ChangLastPlayerDisplay() {
        UIManager.Instance.ChangCurrentPlayerID(-1);
    }
}
