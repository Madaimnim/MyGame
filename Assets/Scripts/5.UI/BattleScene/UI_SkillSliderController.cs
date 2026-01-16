using System.Collections.Generic;
using UnityEngine;

public class UI_SkillSliderController : MonoBehaviour {
    [Header("Skill Slot UI Items (¶¶§Ç = Slot Index)")]
    public CooldownSlider[] Sliders = new CooldownSlider[4];

    private CombatComponent _combatComponent;
    private Dictionary<int, ISkillRuntime> _skillPool;

    public void BindCombatComponent(CombatComponent component, Dictionary<int, ISkillRuntime> pool) {
        _combatComponent = component;
        _skillPool = pool;

        RefreshSkillInfo();
    }

    private void RefreshSkillInfo() {
        if (_combatComponent == null) return;

        for (int i = 0; i < Sliders.Length; i++) {
            if (Sliders[i] == null) continue;

            SkillSlot slot = _combatComponent.SkillSlots[i];

            if (!slot.HasSkill || !_skillPool.TryGetValue(slot.SkillId, out var rt)) {
                Sliders[i].UpdateSkillInfo(null);
                continue;
            }

            Sliders[i].UpdateSkillInfo(rt);
        }
    }

    private void Update() {
        if (_combatComponent == null) return;

        for (int i = 0; i < Sliders.Length; i++) {
            SkillSlot slot = _combatComponent.SkillSlots[i];

            if (!slot.HasSkill || !_skillPool.TryGetValue(slot.SkillId, out var rt)) {
                Sliders[i].UpdateCooldown(0f, 1f);
                continue;
            }

            Sliders[i].UpdateCooldown(
                slot.CooldownTimer,
                rt.Cooldown
            );
        }
    }
}
