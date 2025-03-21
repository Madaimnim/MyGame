﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillController : MonoBehaviour
{
    public Button[] slotSkillButtons;  // 技能槽4個按鈕
    public TextMeshProUGUI[] slotSkillNames;  // 技能槽技能名稱的 Text

    public GameObject skillSelectionPanel;  // 技能選擇面板
    public GameObject skillSelectionButtonPrefab;  // 技能選擇按鈕的預製體

    private PlayerStateManager.PlayerStats currentPlayer;
    private Vector2 originSkillSelectionPanelPosition;

    private void OnEnable() {
        if (UIManager.Instance == null)
        {
            Debug.LogWarning($"[UISkillController] 啟用，但 UIManager 為空");
            return;
        }

        PlayerStateManager.Instance.playerStatesDtny.TryGetValue(UIManager.Instance.currentPlayerId, out currentPlayer);
        Debug.Log($"[UISkillController] 獲取當前角色：{currentPlayer?.playerName}");

        EventBus.Listen<UICurrentPlayerChangEvent>(OnCurrentPlayerChanged);

        originSkillSelectionPanelPosition = skillSelectionPanel.transform.position;
        skillSelectionPanel.SetActive(false);

        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            int slotIndex = i;
            slotSkillButtons[i].onClick.AddListener(() => ShowAvailableSkills(slotIndex));
        }
        RefreshSkillSlotButtonText();
    }

    private void OnDisable() {
        EventBus.StopListen<UICurrentPlayerChangEvent>(OnCurrentPlayerChanged);
        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            slotSkillButtons[i].onClick.RemoveAllListeners();
        }
    }

    #region 當「當前腳色變更」事件觸發時，更新本地currentPlayer，並刷新技能槽4個Text
    private void OnCurrentPlayerChanged(UICurrentPlayerChangEvent eventData) {
        currentPlayer = eventData.currentPlayer;
        RefreshSkillSlotButtonText();
    }
    #endregion

    #region 更新技能槽4個TEXT
    private void RefreshSkillSlotButtonText() {
        if (currentPlayer == null) return;

        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            if (i >= slotSkillButtons.Length || i >= slotSkillNames.Length)
            {
                Debug.LogError($"❌ [UISkillController] 技能槽索引超出範圍: {i}");
                continue;
            }
            var skill = currentPlayer.GetSkillAtSkillSlot(i);
            slotSkillNames[i].text = skill != null ? skill.skillName : "空";
        }
    }
    #endregion

    #region 顯示可選技能列表
    private void ShowAvailableSkills(int slotIndex) {
        List<PlayerStateManager.PlayerStats.SkillData> availableSkills = new List<PlayerStateManager.PlayerStats.SkillData>();

        skillSelectionPanel.transform.position = originSkillSelectionPanelPosition;

        if (currentPlayer == null) return;

        // 清除舊的技能按鈕
        foreach (Transform child in skillSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 獲取當前角色的所有可用技能（已解鎖但未裝備的）
        foreach (var skillID in currentPlayer.unlockedSkillIDList)
        {
            if (!currentPlayer.equippedSkillIDList.Contains(skillID))
            {
                var skill = currentPlayer.skillPoolDtny[skillID];
                if (skill != null)
                {
                    availableSkills.Add(skill);
                }
            }
        }

        // 創建可選技能按鈕
        foreach (var skill in availableSkills)
        {
            GameObject skillSelectionButtonObj = Instantiate(skillSelectionButtonPrefab, skillSelectionPanel.transform);
            Button skillButton = skillSelectionButtonObj.GetComponent<Button>();
            TextMeshProUGUI skillText = skillSelectionButtonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (skillText != null)
            {
                skillText.text = skill.skillName;
            }

            skillButton.onClick.AddListener(() => EquipSkill(slotIndex, skill.skillID));
        }

        // 顯示技能選擇面板
        skillSelectionPanel.SetActive(availableSkills.Count > 0);
    }
    #endregion

    #region 透過PlayerStateManager的SetSkillAtSlot裝備技能，並刷新4個技能槽的TEXT
    private void EquipSkill(int slotIndex, int skillID) {
        if (currentPlayer == null) return;

        // 直接更新技能
        currentPlayer.SetSkillAtSlot(slotIndex, skillID);

        // 直接觸發 UI 更新（整個技能欄）
        RefreshSkillSlotButtonText();

        skillSelectionPanel.SetActive(false);
    }
    #endregion
}
