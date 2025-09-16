using UnityEngine;
using System.Collections.Generic;

public class UIManager_SkillCooldown : MonoBehaviour
{
    public static UIManager_SkillCooldown Instance;

    [Header("�ޯ�N�oUI Prefab")]
    public UIController_SkillCooldown skillCooldownUIPrefab;
    [Header("�ޯ�UI������ (UI Panel)")]
    public Transform skillUIParent;

    private List<UIController_SkillCooldown> activeCooldownUIs = new List<UIController_SkillCooldown>();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable() {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Event_SkillCooldownChanged += UpdateSkillCooldown;
            EventManager.Instance.Event_SkillInfoChanged += UpdateSkillInfo;

        }
    }

    private void OnDisable() {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Event_SkillCooldownChanged -= UpdateSkillCooldown;
            EventManager.Instance.Event_SkillInfoChanged -= UpdateSkillInfo;

        }
    }

    // ���U���⪺�ޯ�ѡ]��p�|�ӧޯ�^
    #region RegisterPlayerSkills(Player player, Transform parent)
    public void RegisterPlayerSkills(Player player, Transform parent) {
        for (int i = 0; i < player.GetSkillSlotsLength(); i++)
        {
            var slotData = player.GetSkillSlotData(i);
            if (slotData == null || string.IsNullOrEmpty(slotData.skillName)) continue;

            UIController_SkillCooldown ui = Instantiate(skillCooldownUIPrefab, parent);
            ui.Setup(player.GetSkillSlot(i), player.playerAI, i); // �� SkillSlot + PlayerAI
            activeCooldownUIs.Add(ui);
        }
    }
    #endregion

    //�ƥ�q�\
    #region

    // ��sUIController�W��UI�ޯ�N�o
    #region UpdateSkillCooldown(int slotIndex, float current, float max, PlayerAI playerAI)
    private void UpdateSkillCooldown(int slotIndex, float current, float max, PlayerAI playerAI) {
        foreach (var ui in activeCooldownUIs)
        {
            if (ui.IsBoundTo(playerAI, slotIndex))
            {
                ui.UpdateCooldownUI(current, max);
            }
        }
    }
    #endregion

    // ��sUIController�W���ޯ��T
    #region UpdateSkillInfo(int slotIndex, PlayerAI playerAI)
    private void UpdateSkillInfo(int slotIndex, PlayerAI playerAI) {
        foreach (var ui in activeCooldownUIs)
        {
            if (ui.IsBoundTo(playerAI, slotIndex))
            {
                ui.RefreshSkillInfo();
            }
        }
    }
    #endregion

    #endregion
}
