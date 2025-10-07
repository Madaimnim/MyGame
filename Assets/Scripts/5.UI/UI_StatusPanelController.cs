using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class UI_StatusPanelController : UI_PanelBase
{
    public Button CurrentPlayerUpButton;
    public Button CurrentPlayerDownButton;

    public TextMeshProUGUI[] statusTextsArray; // 角色基本狀態顯示（前 6 個）/

    private int currentPlayerId = -1;
    private PlayerStatsRuntime _currentPlayerRuntime;

    private void OnEnable() {
        base.OnEnable();
        CurrentPlayerUpButton.onClick.AddListener(PlayerChangedUp);
        CurrentPlayerDownButton.onClick.AddListener(PlayerChangedDown);
    }

    private void OnDisable() {
        CurrentPlayerUpButton.onClick.RemoveListener(PlayerChangedUp);
        CurrentPlayerDownButton.onClick.RemoveListener(PlayerChangedDown);
    }

    //按鈕綁定方法
    private void PlayerChangedUp() {
        var ids = PlayerUtility.UnlockedIdList;
        if (ids.Count == 0) return;
        int index = ids.IndexOf(currentPlayerId);
        index = (index + 1) % ids.Count;
        currentPlayerId = ids[index];
        _currentPlayerRuntime = PlayerUtility.AllRts[currentPlayerId];
        Refresh();
        UIPlayerActive(currentPlayerId);
    }
    private void PlayerChangedDown() {
        var ids = PlayerUtility.UnlockedIdList.ToList();
        if (ids.Count == 0) return;
        int index = ids.IndexOf(currentPlayerId);
        index = (index - 1 + ids.Count) % ids.Count;
        currentPlayerId = ids[index];
        _currentPlayerRuntime = PlayerUtility.AllRts[currentPlayerId];
        Refresh();
        UIPlayerActive(currentPlayerId);
    }


    public override void Refresh() {
        if (!GameManager.Instance.IsAllDataLoaded) return;
        if (currentPlayerId == -1) {
            currentPlayerId = PlayerUtility.UnlockedIdList.First();
            _currentPlayerRuntime = PlayerUtility.AllRts[currentPlayerId];
        }

        statusTextsArray[0].text = $"等級: {_currentPlayerRuntime.StatsData.Level}";
        statusTextsArray[1].text = $"HP: {_currentPlayerRuntime.MaxHp}/{_currentPlayerRuntime.MaxHp}";
        statusTextsArray[2].text = $"名稱: {_currentPlayerRuntime.StatsData.Name}";
        statusTextsArray[3].text = $"攻擊力: {_currentPlayerRuntime.StatsData.Power}";
        statusTextsArray[4].text = $"經驗值: {_currentPlayerRuntime.Exp}/{_currentPlayerRuntime.Exp}";
        statusTextsArray[5].text = $"速度: {_currentPlayerRuntime.StatsData.MoveSpeed}";
    }

    //每切換當前UI角色執行，將activePlayerUIDtny中的角色物件Active
    public void UIPlayerActive(int playerID) {
        foreach (var kvp in PlayerUtility.AllRts) // 遍歷所有角色 UI
        {
            bool isActive = (kvp.Key == playerID); // 只有當前角色 UI 設為 true
            kvp.Value.BattleObject.SetActive(isActive);
        }
    }

}
