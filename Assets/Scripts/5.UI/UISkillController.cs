using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillController : MonoBehaviour
{
    public Button[] slotSkillButtons;  // 技能槽4個按鈕
    public TextMeshProUGUI[] slotSkillNames;  // 技能槽技能名稱的 Text
    private PlayerStateManager.PlayerStats.SkillData[] displayedSkills = new PlayerStateManager.PlayerStats.SkillData[4];

    public GameObject skillSelectionPanel;  // 技能選擇面板
    public GameObject skillSelectionButtonPrefab;  // 技能選擇按鈕的預製體

    private PlayerStateManager.PlayerStats currentPlayer;
    private Vector2 originSkillSelectionPanelPosition;

    private void OnEnable() {
        EventBus.Listen<UISkillChangeEvent>(OnSkillChanged);
        EventBus.Listen<UICurrentPlayerChangEvent>(OnUICurrentPlayerChanged);


        originSkillSelectionPanelPosition = new Vector2(skillSelectionPanel.transform.position.x, skillSelectionPanel.transform.position.y); 
        skillSelectionPanel.SetActive(false);

        // 監聽技能槽4個按鈕點擊事件，並觸發ShowAvailableSkills(slotIndex))方法，顯示可選擇的更新技能
        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            int slotIndex = i;
            slotSkillButtons[i].onClick.AddListener(() => ShowAvailableSkills(slotIndex));
        }

        if(UIManager.Instance==null)
                return;
        PlayerStateManager.Instance.playerStatesDtny.TryGetValue(UIManager.Instance.currentPlayerId, out currentPlayer);
        RefreshSkillSlotUI(currentPlayer);
    }
    private void OnDisable() {
        EventBus.StopListen<UISkillChangeEvent>(OnSkillChanged);
        EventBus.StopListen<UICurrentPlayerChangEvent>(OnUICurrentPlayerChanged);

        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            slotSkillButtons[i].onClick.RemoveAllListeners();
        }
    }

    #region 監聽者：當「技能變更」事件、「當前腳色變更」事件觸發時，執行該方法
    //當某技能槽技能變更時，執行UpdateSkillButton方法，更新按鈕及文字。
    private void OnSkillChanged(UISkillChangeEvent eventData) {
        displayedSkills[eventData.slotIndex] = eventData.newSkill;
        UpdateSkillButton(eventData.slotIndex, eventData.newSkill);
    }
    //UIManager更新currentPlayer時，此類中的currentPlayer跟著變更
    private void OnUICurrentPlayerChanged(UICurrentPlayerChangEvent eventData) {
        currentPlayer = eventData.currentPlayer;  
    }
    #endregion

    #region 根據當前腳色及其裝備槽上的技能，更新displayeredSkill[]，提供給UpdateSkillbutton()方法，並更新技能按鈕及文字
    private void RefreshSkillSlotUI(PlayerStateManager.PlayerStats currentPlayer) {
        this.currentPlayer = currentPlayer;
        if (this.currentPlayer == null) return;
        for (int i = 0; i < displayedSkills.Length; i++)
        {
            displayedSkills[i] = this.currentPlayer.GetSkillAtSkillSlot(i);
            UpdateSkillButton(i, displayedSkills[i]);
        }
    }
    #endregion
    #region UpdateSkillButton方法：根據技能槽及技能資料，更新技能按鈕可互動性、文字名稱。
    private void UpdateSkillButton(int index, PlayerStateManager.PlayerStats.SkillData skill) {
        if (index < 0 || index >= slotSkillButtons.Length || index >= slotSkillNames.Length)
        {
            Debug.LogError($"❌ [UISkillController] 技能槽索引超出範圍: {index}");
            return;
        }

        if (skill != null)
        {
            slotSkillNames[index].text = skill.skillName;
        }
        else
        {
            slotSkillNames[index].text = "空";
        }
    }
    #endregion

    #region 顯示及創建可選技能清單
    private void ShowAvailableSkills(int slotIndex) {
        List<PlayerStateManager.PlayerStats.SkillData> availableSkills = new List<PlayerStateManager.PlayerStats.SkillData>();

        skillSelectionPanel.transform.position = new Vector2(originSkillSelectionPanelPosition.x, originSkillSelectionPanelPosition.y);

        if (currentPlayer == null) return;
        // 清除舊的技能選擇按鈕
        foreach (Transform child in skillSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 獲取當前角色的所有可用技能（已解鎖但未裝備的）
        
        foreach (var skillID in currentPlayer.unlockedSkillIDList)
        {
            // 確保該技能未被裝備
            if (!currentPlayer.equippedSkillIDList.Contains(skillID))
            {
                // 從 skillPoolList 中尋找對應的技能數據
                var skill = currentPlayer.skillPoolDtny[skillID];
                if (skill != null)
                {
                    availableSkills.Add(skill);
                }
                else
                {
                    Debug.LogWarning($"[PlayerStateManager] 在 skillPoolList 找不到技能 ID: {skillID}，但它存在於 unlockedSkillIDList 中");
                }
            }
        }

        // 創建按鈕來選擇技能
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
        if (currentPlayer.unlockedSkillIDList.Count > 0)
        {
            skillSelectionPanel.SetActive(true);

            int extraButtons = availableSkills.Count - 7;
            if (extraButtons > 0)
            {
                RectTransform panelRect = skillSelectionPanel.GetComponent<RectTransform>();
                Vector3 newPos = panelRect.anchoredPosition;
                newPos.y += extraButtons * 70; // 每多一個按鈕，往上移動 70 單位
                panelRect.anchoredPosition = newPos;
            }
        }
        else
            skillSelectionPanel.SetActive(false);
    }

    private void EquipSkill(int slotIndex, int skillID) {
        if (currentPlayer == null) return;

        EventBus.Trigger(new EquipSkillEvent(slotIndex, skillID));  // 觸發裝備事件
        skillSelectionPanel.SetActive(false);  // 關閉技能選擇面板
    }
    #endregion
}
