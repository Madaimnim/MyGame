using System.Collections.Generic;
using UnityEngine;

public class EnergyGainSystem {
    private readonly List<IEnergyGainRule> _rules = new();

    public void AddRule(IEnergyGainRule rule) {
        _rules.Add(rule);
    }

    public void HandleSkillUsed(ISkillRuntime skill) {
        foreach (var energyRule in _rules) {
            energyRule.GainEnergy(skill);
        }

    }
}
