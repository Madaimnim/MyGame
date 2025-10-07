using UnityEngine;
using System.Collections.Generic;

public class UI_SkillCooldownPanelController : MonoBehaviour
{
    [Header("技能冷卻UI Prefab")]
    public UI_SkillCooldownSlider SliderPrefab;
    [Header("技能UI父物件 (UI Panel)")]
    public Transform SlidersParent;

    private Player _player;  // 綁定的角色
    private List<UI_SkillCooldownSlider> _cooldownSliderList = new List<UI_SkillCooldownSlider>();


    private void OnEnable() {
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Event_SkillCooldownChanged += UpdateSkillCooldown;
            GameEventSystem.Instance.Event_SkillInfoChanged += UpdateSkillInfo;

        }
    }
    private void OnDisable() {
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Event_SkillCooldownChanged -= UpdateSkillCooldown;
            GameEventSystem.Instance.Event_SkillInfoChanged -= UpdateSkillInfo;

        }
    }

    //註冊角色的技能槽
    public void RegisterPlayerSkillSliders(Player player) {
        _player = player;
        for (int slotIndex = 0; slotIndex < player.Rt.SkillSlotCount; slotIndex++)
        {
            if (!_player.SkillComponent.SkillSlots[slotIndex].HasSkill) continue;
            
            var cooldownSlider = Instantiate(SliderPrefab, SlidersParent);
            cooldownSlider.Setup(slotIndex, player);
            _cooldownSliderList.Add(cooldownSlider);
        }
    }

    // 更新UISlot上的UI技能冷卻
    public void UpdateSkillCooldown(int slotIndex, float current, float max, Player player) {
        if (_player == null || _player != player) return; // 過濾不是我的角色

        foreach (var slider in _cooldownSliderList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.UpdateCooldownUI(current, max);
    }

    // 更新UIController上的技能資訊

    public void UpdateSkillInfo(int slotIndex, Player player) {
        if (_player == null || _player != player) return;

        foreach (var slider in _cooldownSliderList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.RefreshSkillInfo();
    }
}
