using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController_Skill : MonoBehaviour
{
    [Header("狀態欄")]
    public Button[] slotSkillButtons;  // 技能槽4個按鈕
    public TextMeshProUGUI[] slotSkillNames;  // 技能槽技能名稱的 Text
    public GameObject skillSelectionPanel;  // 技能選擇面板
    public GameObject skillSelectionButtonPrefab;  // 技能選擇按鈕的預製體

    private PlayerStatsRuntime currentPlayer;
    private Vector2 originSkillSelectionPanelPosition;



    //生命週期
    #region 生命週期
    private void OnEnable() {
        GameEventSystem.Instance.Event_SkillEquipped += OnSkillEquippedUI;
        GameEventSystem.Instance.Event_SkillUnlocked += OnSkillUnlockedUI;

        EventBus.Listen<UICurrentPlayerChangEvent>(OnCurrentPlayerChanged);
        StartCoroutine(WaitAndInit());
    }

    private void OnDisable() {
        GameEventSystem.Instance.Event_SkillEquipped -= OnSkillEquippedUI;
        GameEventSystem.Instance.Event_SkillUnlocked -= OnSkillUnlockedUI;

        EventBus.StopListen<UICurrentPlayerChangEvent>(OnCurrentPlayerChanged);
        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            slotSkillButtons[i].onClick.RemoveAllListeners();
        }
    }
    #endregion


    private IEnumerator WaitAndInit() {
        yield return new WaitUntil(() => UIManager.Instance != null && GameManager.Instance.PlayerSystem != null);
        GameManager.Instance.PlayerSystem.TryGetStatsRuntime(UIManager.Instance.currentPlayerId, out currentPlayer);
        originSkillSelectionPanelPosition = skillSelectionPanel.transform.position;
        skillSelectionPanel.SetActive(false);
        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            int slotIndex = i;
            slotSkillButtons[i].onClick.AddListener(() => ShowAvailableSkills(slotIndex));
        }
        RefreshSkillSlotButtonText();
    }
    
    //Todo
    private void OnSkillEquippedUI(int id,int slotIndex, PlayerSkillRuntime skillRuntime) {
        if (currentPlayer == null) return;
        var skill = currentPlayer.GetSkillRuntimeAtSlot(slotIndex);
        slotSkillNames[slotIndex].text = skill != null ? skill.SkillName : "空";
    }
    //Todo
    private void OnSkillUnlockedUI(int playerId, int skillId) {
        // 例如刷新技能清單
        if (currentPlayer != null && currentPlayer.StatsData.Id == playerId)
        {
            RefreshSkillSlotButtonText();
        }
    }


    private void OnCurrentPlayerChanged(UICurrentPlayerChangEvent eventData) {
        currentPlayer = eventData.currentPlayer;
        RefreshSkillSlotButtonText();
    }
    private void RefreshSkillSlotButtonText() {
        if (currentPlayer == null) return;

        for (int i = 0; i < slotSkillButtons.Length; i++)
        {
            if (i >= slotSkillButtons.Length || i >= slotSkillNames.Length)
            {
                Debug.LogError($"❌ [UIController_Skill] 技能槽索引超出範圍: {i}");
                continue;
            }
            var skill = currentPlayer.GetSkillRuntimeAtSlot(i);
            slotSkillNames[i].text = skill != null ? skill.SkillName : "空";
        }
    }

    private void ShowAvailableSkills(int slotIndex) {
        List<PlayerSkillRuntime> availableSkills = new List<PlayerSkillRuntime>();

        skillSelectionPanel.transform.position = originSkillSelectionPanelPosition;

        if (currentPlayer == null) return;

        // 清除舊的技能按鈕
        foreach (Transform child in skillSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // 獲取當前角色的所有可用技能（已解鎖但未裝備的）
        foreach (var skillID in currentPlayer.UnlockedSkillIdList)
        {
            if (!currentPlayer.HasEquippedSkill(skillID))
            {
                var skill = currentPlayer.PlayerSkillRuntimeDtny[skillID];
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
                skillText.text = skill.SkillName;
            }

            skillButton.onClick.AddListener(() => EquipSkill(slotIndex, skill.SkillId));
        }

        // 顯示技能選擇面板
        skillSelectionPanel.SetActive(availableSkills.Count > 0);
    }
    private void EquipSkill(int slotIndex, int skillID) {
        if (currentPlayer == null) return;

        // 直接更新技能
        currentPlayer.SkillSystem.EquipSkill(currentPlayer.StatsData.Id, slotIndex, skillID);

        // 直接觸發 UI 更新（整個技能欄）
        RefreshSkillSlotButtonText();

        skillSelectionPanel.SetActive(false);
    }

}
