using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillUseGate : ISkillUseGate {
    private readonly EnergyComponent _energyComponent;

    public PlayerSkillUseGate(EnergyComponent energyComponent) {
        _energyComponent = energyComponent;
    }

    public bool CanUse(ISkillRuntime skillRt, int slotIndex) {
        //Debug.Log($"Skill {skillRt.Id} with cost {skillRt.EnergyCost}， Current energy: {_energyComponent.CurrentEnergy}");
        return _energyComponent.HasEnoughEnergy(skillRt.EnergyCost);
    }

    public void Consume(ISkillRuntime skillRt, int slotIndex) {
        //Debug.Log($"Consume skill {skillRt.Id} energy cost {skillRt.EnergyCost}");
        _energyComponent.Consume(skillRt.EnergyCost);

        // 未來加：
        // - 第三段技能判斷
        // - 同招衰減紀錄
        // - Build 專屬回饋
    }
}
