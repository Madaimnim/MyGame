using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController_SkillCooldown : MonoBehaviour
{
    [Header("UI 組件")]
    public Slider cooldownSlider;
    public TMP_Text skillNameText;
    public TMP_Text cooldownText;

    private PlayerAI boundPlayerAI;
    private int boundSlotIndex;
    private Player.SkillSlot boundSlot; // 綁定 SkillSlot 直接讀取資訊

    // 初始化
    #region Setup(string skillName, Player player, int slotIndex)
    public void Setup(Player.SkillSlot slot, PlayerAI playerAI, int slotIndex) {
        boundPlayerAI = playerAI;
        boundSlotIndex = slotIndex;
        boundSlot = slot;

        if (skillNameText != null && boundSlot.skillData != null)
        {
            skillNameText.text = $"{boundSlot.skillData.skillName} Lv.{boundSlot.skillData.currentLevel}";
        }
    }
    #endregion

    // 檢查是否對應某個角色與技能槽
    #region IsBoundTo(Player player, int slotIndex)
    public bool IsBoundTo(PlayerAI playerAI, int slotIndex) {
        return boundPlayerAI == playerAI && boundSlotIndex == slotIndex;
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
        if (boundSlot != null && boundSlot.skillData != null && skillNameText != null)
        {
            skillNameText.text = $"{boundSlot.skillData.skillName} Lv.{boundSlot.skillData.currentLevel}";
        }
    }
    #endregion
}
