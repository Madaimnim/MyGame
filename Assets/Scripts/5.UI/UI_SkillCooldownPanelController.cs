using UnityEngine;
using System.Collections.Generic;

public class UI_SkillCooldownPanelController : MonoBehaviour
{
    [Header("技能冷卻UI Prefab")]
    public UI_SkillCooldownSlider skillCooldownSliderPrefab;
    [Header("技能UI父物件 (UI Panel)")]
    public Transform skillSliderParent;

    private Player boundPlayer;  // 綁定的角色
    private List<UI_SkillCooldownSlider> activeCooldownSlotList = new List<UI_SkillCooldownSlider>();


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

    //註冊角色的技能槽（比如四個技能）
    #region RegisterPlayerSkillSlider(Player player, Transform parent)
    public void RegisterPlayerSkillSliders(Player player) {
        boundPlayer = player;
        for (int slotIndex = 0; slotIndex < player.GetSkillSlotsLength(); slotIndex++)
        {
            var slotData = player.StatsRuntime.GetSkillDataRuntimeAtSlot(slotIndex);
            if (slotData == null || string.IsNullOrEmpty(slotData.SkillName)) continue;
            //生成Slider
            var cooldownSlot = Instantiate(skillCooldownSliderPrefab, skillSliderParent);
            cooldownSlot.Setup(slotIndex, player);
            activeCooldownSlotList.Add(cooldownSlot);
        }
    }
    #endregion


    // 更新UISlot上的UI技能冷卻
    #region
    public void UpdateSkillCooldown(int slotIndex, float current, float max, Player player) {
        if (boundPlayer == null || player != boundPlayer.playerAI) return; // 過濾不是我的角色

        foreach (var slider in activeCooldownSlotList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.UpdateCooldownUI(current, max);
    }
    #endregion

    // 更新UIController上的技能資訊
    #region UpdateSkillInfo(int slotIndex, PlayerAI playerAI)
    public void UpdateSkillInfo(int slotIndex, Player player) {
        if (boundPlayer == null || player != boundPlayer.playerAI) return;

        foreach (var slider in activeCooldownSlotList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.RefreshSkillInfo();
    }
    #endregion
}
