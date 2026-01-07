using UnityEngine;

public class Action_AttackAllSlots : Node
{
    private AIComponent _ai;
    private SkillComponent _skillComponent;

    public Action_AttackAllSlots(AIComponent ai) {
        _ai = ai;
        _skillComponent= _ai.SkillComponent;

    }

    public override NodeState Evaluate(float updateInterval) {
        for (int i = 0; i < _skillComponent.SkillSlotCount; i++)
        {
            var slot = _skillComponent.SkillSlots[i];
            if (slot == null || slot.SkillId ==-1) continue;
            if (!slot.IsReady || slot.Detector == null || !slot.Detector.HasTarget) continue;

            _skillComponent.SetIntentSkill(i, slot.Detector.TargetTransform.position, slot.Detector.TargetTransform);
            return NodeState.SUCCESS;
        }

        _skillComponent.ClearAllSkillIntent();
        return NodeState.FAILURE; // 沒有任何技能能用
    }
}
