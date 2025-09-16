using UnityEngine;
using System.Collections.Generic;

public class UIManager_SkillCooldown : MonoBehaviour
{
    public static UIManager_SkillCooldown Instance;

    [Header("技能冷卻UI Prefab")]
    public UIController_SkillCooldown skillCooldownUIPrefab;
    [Header("技能UI父物件 (UI Panel)")]
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

    // 註冊角色的技能槽（比如四個技能）
    #region RegisterPlayerSkills(Player player, Transform parent)
    public void RegisterPlayerSkills(Player player, Transform parent) {
        for (int i = 0; i < player.GetSkillSlotsLength(); i++)
        {
            var slotData = player.GetSkillSlotData(i);
            if (slotData == null || string.IsNullOrEmpty(slotData.skillName)) continue;

            UIController_SkillCooldown ui = Instantiate(skillCooldownUIPrefab, parent);
            ui.Setup(player.GetSkillSlot(i), player.playerAI, i); // 傳 SkillSlot + PlayerAI
            activeCooldownUIs.Add(ui);
        }
    }
    #endregion

    //事件訂閱
    #region

    // 更新UIController上的UI技能冷卻
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

    // 更新UIController上的技能資訊
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
