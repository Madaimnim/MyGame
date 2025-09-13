using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager_Player : MonoBehaviour
{
    public static UIManager_Player Instance;

    [Header("���a UI ������ (UI Panel)")]
    public Transform playerUIParent;
    [Header("���a UI �e�� Prefab")]
    public UIController_Player playerUIPrefab;

    private List<UIController_Player> activePlayerUIs = new List<UIController_Player>();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //���U���a
    #region RegisterPlayer(Player player)
    public void RegisterPlayer(Player player) {

        UIController_Player ui = Instantiate(playerUIPrefab, playerUIParent);
        ui.Setup(player);
        activePlayerUIs.Add(ui);
        Debug.Log($"{player.playerStats.playerName}���U�F");

        //  �j���s Layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(playerUIParent as RectTransform);
    }
    #endregion



    //���P���a
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