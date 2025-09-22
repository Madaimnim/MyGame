using UnityEngine;
using System.Collections.Generic;

public class UI_SkillCooldownPanelController : MonoBehaviour
{
    [Header("�ޯ�N�oUI Prefab")]
    public UI_SkillCooldownSlider skillCooldownSliderPrefab;
    [Header("�ޯ�UI������ (UI Panel)")]
    public Transform skillSliderParent;

    private Player boundPlayer;  // �j�w������
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

    //���U���⪺�ޯ�ѡ]��p�|�ӧޯ�^
    #region RegisterPlayerSkillSlider(Player player, Transform parent)
    public void RegisterPlayerSkillSliders(Player player) {
        boundPlayer = player;
        for (int slotIndex = 0; slotIndex < player.GetSkillSlotsLength(); slotIndex++)
        {
            var slotData = player.StatsRuntime.GetSkillDataRuntimeAtSlot(slotIndex);
            if (slotData == null || string.IsNullOrEmpty(slotData.SkillName)) continue;
            //�ͦ�Slider
            var cooldownSlot = Instantiate(skillCooldownSliderPrefab, skillSliderParent);
            cooldownSlot.Setup(slotIndex, player);
            activeCooldownSlotList.Add(cooldownSlot);
        }
    }
    #endregion


    // ��sUISlot�W��UI�ޯ�N�o
    #region
    public void UpdateSkillCooldown(int slotIndex, float current, float max, Player player) {
        if (boundPlayer == null || player != boundPlayer.playerAI) return; // �L�o���O�ڪ�����

        foreach (var slider in activeCooldownSlotList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.UpdateCooldownUI(current, max);
    }
    #endregion

    // ��sUIController�W���ޯ��T
    #region UpdateSkillInfo(int slotIndex, PlayerAI playerAI)
    public void UpdateSkillInfo(int slotIndex, Player player) {
        if (boundPlayer == null || player != boundPlayer.playerAI) return;

        foreach (var slider in activeCooldownSlotList)
            if (slider.IsBoundTo(player, slotIndex))
                slider.RefreshSkillInfo();
    }
    #endregion
}
