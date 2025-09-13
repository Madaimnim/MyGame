using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController_SkillCooldown : MonoBehaviour
{
    [Header("UI 組件")]
    public Slider cooldownSlider;
    public TMP_Text skillNameText;
    public TMP_Text cooldownText;

    private Player boundPlayer;
    private int boundSlotIndex;
    private Player.SkillSlot boundSlot; // 綁定 SkillSlot 直接讀取資訊

    // 初始化
    #region Setup(string skillName, Player player, int slotIndex)
    public void Setup(Player.SkillSlot slot, Player player, int slotIndex) {
        boundPlayer = player;
        boundSlotIndex = slotIndex;
        boundSlot = slot;

        if (skillNameText != null)
            skillNameText.text = $"{player.GetSkillName(slotIndex)} Lv.{player.GetSkillLevel(slotIndex)}";
    }
    #endregion

    // 檢查是否對應某個角色與技能槽
    #region IsBoundTo(Player player, int slotIndex)
    public bool IsBoundTo(Player player, int slotIndex) {
        return boundPlayer == player && boundSlotIndex == slotIndex;
    }
    #endregion

    // 更新冷卻 UI
    #region UpdateCooldownUI(float current, float max)
    public void UpdateCooldownUI(float current, float max) {
        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = max;
            cooldownSlider.value = max-current;
        }

        if (cooldownText != null)
        {
            cooldownText.text = current > 0 ? $"{current:F1}s" : "Ready";
        }
    }
    #endregion

    //更新SkillName、Level
    #region RefreshSkillInfo()
    public void RefreshSkillInfo() {
        if (boundPlayer != null && skillNameText != null)
        {
            skillNameText.text = $"{boundPlayer.GetSkillName(boundSlotIndex)} Lv.{boundPlayer.GetSkillLevel(boundSlotIndex)}";
        }
    }
    #endregion
}
