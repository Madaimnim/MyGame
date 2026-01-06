using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownSlider : MonoBehaviour {
    [Header("UI")]
    public TMP_Text SkillNameText;
    public TMP_Text CooldownText;
    public Image SkillIcon;
    public Image CooldownMask;

    public void UpdateCooldown(float current, float max) {
        if (CooldownMask != null && max > 0f) {
            CooldownMask.fillAmount = current / max;
        }

        if (CooldownText != null) {
            CooldownText.text = current > 0 ? $"{current:F1}s" : " ";
        }
    }

    public void UpdateSkillInfo(ISkillRuntime skillRt) {
        if (SkillNameText != null)
            SkillNameText.text = skillRt != null ? skillRt.StatsData.Name : "空";
    }
}
