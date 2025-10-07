using UnityEngine;
using System.Collections.Generic;

public class UI_SkillCooldownPanelController : MonoBehaviour
{
    [Header("�ޯ�N�oUI Prefab")]
    public UI_SkillCooldownSlider SliderPrefab;
    [Header("�ޯ�UI������ (UI Panel)")]
    public Transform SlidersParent;

    private Player _player;  // �j�w������
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

    //���U���⪺�ޯ��
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

    // ��sUISlot�W��UI�ޯ�N�o
    public void UpdateSkillCooldown(int slotIndex, float current, float max, Player player) {
        if (_player == null || _player != player) return; // �L�o���O�ڪ�����

        foreach (var slider in _cooldownSliderList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.UpdateCooldownUI(current, max);
    }

    // ��sUIController�W���ޯ��T

    public void UpdateSkillInfo(int slotIndex, Player player) {
        if (_player == null || _player != player) return;

        foreach (var slider in _cooldownSliderList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.RefreshSkillInfo();
    }
}
