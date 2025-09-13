using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager_Player : MonoBehaviour
{
    public static UIManager_Player Instance;

    [Header("玩家 UI 父物件 (UI Panel)")]
    public Transform playerUIParent;
    [Header("玩家 UI 容器 Prefab")]
    public UIController_Player playerUIPrefab;

    private List<UIController_Player> activePlayerUIs = new List<UIController_Player>();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //註冊玩家
    #region RegisterPlayer(Player player)
    public void RegisterPlayer(Player player) {

        UIController_Player ui = Instantiate(playerUIPrefab, playerUIParent);
        ui.Setup(player);
        activePlayerUIs.Add(ui);
        Debug.Log($"{player.playerStats.playerName}註冊了");

        //  強制刷新 Layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerUIParent as RectTransform);
    }
    #endregion



    //註銷玩家
    #region UnregisterPlayer(Player player)
    public void UnregisterPlayer(Player player) {
        for (int i = activePlayerUIs.Count - 1; i >= 0; i--)
        {
            if (activePlayerUIs[i] != null && activePlayerUIs[i].IsBoundTo(player))
            {
                Destroy(activePlayerUIs[i].gameObject);
                activePlayerUIs.RemoveAt(i);
            }
        }
    }
    #endregion
}