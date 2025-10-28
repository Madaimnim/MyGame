using UnityEngine;

public class Action_AttackAllSlots : Node
{
    private SkillComponent _skillComponent;
    private AIComponent _aiComponent;

    public Action_AttackAllSlots(SkillComponent skillComponent, AIComponent aiComponent) {
        _skillComponent = skillComponent;
        _aiComponent = aiComponent;
    }

    public override NodeState Evaluate(float updateInterval) {
        for (int i = 0; i < _skillComponent.SkillSlotCount; i++)
        {
            var slot = _skillComponent.SkillSlots[i];
            if (slot == null || slot.SkillId ==-1) continue;
            if (!slot.IsReady || slot.DetectStrategy == null || !slot.DetectStrategy.HasTarget) continue;
     
            _aiComponent.SetIntentSkill(_skillComponent,i, slot.DetectStrategy.TargetTransform.position, slot.DetectStrategy.TargetTransform);
            return NodeState.SUCCESS;
        }

        _aiComponent.SetIntentSkill(_skillComponent ,- 1);
        return NodeState.FAILURE; // �S������ޯ���
    }
}
