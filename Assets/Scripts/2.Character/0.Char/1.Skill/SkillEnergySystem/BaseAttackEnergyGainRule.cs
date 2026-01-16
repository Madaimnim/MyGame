using UnityEngine;

public class BaseAttackEnergyGainRule : IEnergyGainRule {
    private readonly EnergyComponent _energyComponent;

    public BaseAttackEnergyGainRule(EnergyComponent energy) {
        _energyComponent = energy;
    }

    public void GainEnergy(ISkillRuntime skillRt) {
        if (skillRt.Id != 1) return;
        Debug.Log($"´¶§ð«ì´_¯à¶q{skillRt.EnergyGain}");
        _energyComponent.Gain(skillRt.EnergyGain); 
    }
}
