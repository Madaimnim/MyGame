using UnityEngine;

public class Action_AttackAllSlots : Node
{
    private AIComponent _ai;
    private CombatComponent _combatComponent;

    public Action_AttackAllSlots(AIComponent ai) {
        _ai = ai;
        _combatComponent= _ai.CombatComponent;

    }

    public override NodeState Evaluate(float updateInterval) {
        for (int i = 0; i < _combatComponent.SkillSlotCount; i++)
        {
            var slot = _combatComponent.SkillSlots[i];
            if(slot == null || !slot.IsReady) continue;
            if (slot.Detector == null || !slot.Detector.HasTarget) continue;

            if(_combatComponent.SetIntentSkill(i, slot.Detector.TargetTransform.position, slot.Detector.TargetTransform))
                return NodeState.SUCCESS;
            else
                continue;
        }

        return NodeState.FAILURE; // 沒有任何技能能用
    }
}
