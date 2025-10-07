using UnityEngine;

public class Action_AttackAllSlots : Node
{
    private SkillComponent _skillComponent;


    public Action_AttackAllSlots(SkillComponent skillComponent) {
        _skillComponent = skillComponent;
    }

    public override NodeState Evaluate() {
        for (int i = 0; i < _skillComponent.SkillSlotCount; i++)
        {
            var slot = _skillComponent.SkillSlots[i];
            if (slot == null || slot.SkillId ==-1) continue;

            if (_skillComponent.CanUseSkill(i))
            {
                _skillComponent.PlaySkillAnimation(i);
                return NodeState.SUCCESS;
            }
        }

        return NodeState.FAILURE; // 沒有任何技能能用
    }
}
