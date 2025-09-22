using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager_BattlePlayer : MonoBehaviour
{
    public static UIManager_BattlePlayer Instance { get; private set; }

    [Header("玩家 UI 父物件 (UI Panel)")]
    public Transform playerUIParent;
    [Header("玩家 UI 容器 Prefab")]
    public UI_BattlePlayerPanel playerUIPrefab;

    private List<UI_BattlePlayerPanel> activePlayerPanelList = new List<UI_BattlePlayerPanel>();

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    //註冊玩家
    #region RegisterPlayer(Player player)
    public void RegisterPlayerPanelUI(Player player) {
        UI_BattlePlayerPanel battlePlayerPanel = Instantiate(playerUIPrefab, playerUIParent);
        battlePlayerPanel.Setup(player);
        activePlayerPanelList.Add(battlePlayerPanel);

        //  強制刷新 Layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerUIParent as RectTransform);
    }
    #endregion

    //註銷玩家
    #region UnregisterPlayer(Player player)
    public void UnregisterPlayer(Player player) {
        for (int i = activePlayerPanelList.Count - 1; i >= 0; i--)
        {
            if (activePlayerPanelList[i] != null && activePlayerPanelList[i].IsBoundTo(player))
            {
                Destroy(activePlayerPanelList[i].gameObject);
                activePlayerPanelList.RemoveAt(i);
            }
        }
    }
    #endregion
}