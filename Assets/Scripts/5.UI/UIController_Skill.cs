using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIController_Skill : MonoBehaviour
{
    [Header("狀態欄")]
    public Button[] SlotSkillButtons;  
    public TextMeshProUGUI[] SlotSkillNames;  // 技能槽技能名稱的 Text
    public GameObject SkillSelectionPanel;  // 技能選擇面板
    public GameObject SkillSelectionButtonPrefab;  // 技能選擇按鈕的預製體

    private PlayerStatsRuntime _currentRt;
    private Vector2 originSkillSelectionPanelPosition;

    //Todo
    private int currentPlayerId=-1;

    //生命週期
    #region 生命週期
    private void OnEnable() {
        GameEventSystem.Instance.Event_SkillUnlocked += OnSkillUnlockedUI;

        GameEventSystem.Instance.Event_UICurrentPlayerChanged += OnUICurrentPlayerChanged;
        StartCoroutine(WaitAndInit());
    }

    private void OnDisable() {
        GameEventSystem.Instance.Event_SkillUnlocked -= OnSkillUnlockedUI;

        GameEventSystem.Instance.Event_UICurrentPlayerChanged += OnUICurrentPlayerChanged;

        for (int i = 0; i < SlotSkillButtons.Length; i++)
        {
            SlotSkillButtons[i].onClick.RemoveAllListeners();
        }
    }
    #endregion


    private IEnumerator WaitAndInit() {
        yield return new WaitUntil(() => UIManager.Instance != null && GameManager.Instance.PlayerStateSystem != null);
        if (currentPlayerId==-1)
        {
            currentPlayerId = PlayerUtility.UnlockedIdList.First();
        }
        var _currentRt=PlayerUtility.Get(currentPlayerId);
        originSkillSelectionPanelPosition = SkillSelectionPanel.transform.position;
        SkillSelectionPanel.SetActive(false);
        for (int i = 0; i < SlotSkillButtons.Length; i++)
        {
            int slotIndex = i;
            SlotSkillButtons[i].onClick.AddListener(() => ShowAvailableSkills(slotIndex));
        }
        RefreshSkillSlotButtonText();
    }

    private void OnSkillUnlockedUI(int playerId, int skillId) {
        // 例如刷新技能清單
        if (_currentRt != null && _currentRt.StatsData.Id == playerId)
        {
            RefreshSkillSlotButtonText();
        }
    }

    //事件方法
    private void OnUICurrentPlayerChanged(PlayerStatsRuntime Runtime) {
        _currentRt = Runtime;
        RefreshSkillSlotButtonText();
    }
    private void RefreshSkillSlotButtonText() {
        if (_currentRt == null) return;

        for (int slotIndex = 0; slotIndex < SlotSkillButtons.Length; slotIndex++)
        {
            if (slotIndex >= SlotSkillNames.Length) continue;
            var skillId = _currentRt.BattleObject.GetComponent<Player>().SkillComponent.SkillSlots[slotIndex].SkillId;
            if(_currentRt.SkillPool.TryGetValue(skillId,out var skill))
                SlotSkillNames[slotIndex].text = skill != null ? skill.StatsData.Name : "空";

        }
    }


    private void ShowAvailableSkills(int slotIndex) {
        List<ISkillRuntime> availableSkills = new List<ISkillRuntime>();

        SkillSelectionPanel.transform.position = originSkillSelectionPanelPosition;

        if (_currentRt == null) return;

        // 清除舊的技能按鈕
        foreach (Transform child in SkillSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var unlockedSkillID in _currentRt.UnlockedSkillIdList)
            if(!_currentRt.BattleObject.GetComponent<Player>().SkillComponent.SkillSlots.Any(s => s.SkillId == unlockedSkillID))
                if(_currentRt.SkillPool.TryGetValue(unlockedSkillID,out var skill))
                    availableSkills.Add(skill);




        // 創建可選技能按鈕
        foreach (var skill in availableSkills)
        {
            GameObject skillSelectionButtonObj = Instantiate(SkillSelectionButtonPrefab, SkillSelectionPanel.transform);
            Button skillButton = skillSelectionButtonObj.GetComponent<Button>();
            TextMeshProUGUI skillText = skillSelectionButtonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (skillText != null)
            {
                skillText.text = skill.StatsData.Name;
            }

            skillButton.onClick.AddListener(() => EquipSkill(slotIndex, skill.StatsData.Id));
        }

        // 顯示技能選擇面板
        SkillSelectionPanel.SetActive(availableSkills.Count > 0);
    }
    private void EquipSkill(int slotIndex, int skillID) {
        GameManager.Instance.PlayerStateSystem.SkillSystem.EquipPlayerSkill(_currentRt.StatsData.Id, slotIndex, skillID);

        RefreshSkillSlotButtonText();
        SkillSelectionPanel.SetActive(false);
    }

}
