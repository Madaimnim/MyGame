using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillUseGate : ISkillUseGate {

    public EnemySkillUseGate() {
    }

    public bool CanUse(ISkillRuntime skill, int inputSlotNumber) {
        return true; // 現階段永遠允許
    }

    public void Consume(ISkillRuntime skill, int inputSlotNumber) {
        // 不做任何事
    }
}
