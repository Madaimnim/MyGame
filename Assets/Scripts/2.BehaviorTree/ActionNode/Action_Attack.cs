using UnityEngine;

public class Action_Attack : Node
{
    private IAttackable attacker;
    private int skillSlot;
    public Action_Attack(IAttackable attacker, int skillSlot) {
        this.attacker = attacker;
        this.skillSlot = skillSlot;
    }

    public override NodeState Evaluate() {
        if (!attacker.CanUseSkill(skillSlot))
            return NodeState.FAILURE;

        attacker.UseSkill(skillSlot);
        return NodeState.SUCCESS;
    }
}
