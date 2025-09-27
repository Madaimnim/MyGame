using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager_BattlePlayer : MonoBehaviour
{
    public static UIManager_BattlePlayer Instance { get; private set; }

    [Header("���a UI ������ (UI Panel)")]
    public Transform playerUIParent;
    [Header("���a UI �e�� Prefab")]
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

    //���U���a
    #region RegisterPlayer(Player player)
    public void RegisterPlayerPanelUI(Player player) {
        UI_BattlePlayerPanel battlePlayerPanel = Instantiate(playerUIPrefab, playerUIParent);
        battlePlayerPanel.Setup(player);
        activePlayerPanelList.Add(battlePlayerPanel);

        //  �j���s Layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerUIParent as RectTransform);
    }
    #endregion

    //���P���a
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