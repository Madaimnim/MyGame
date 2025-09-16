using UnityEngine;

public class Action_Attack : Node
{
    private IAttackable attacker;
    private int slotIndex;
    public Action_Attack(IAttackable attacker, int slotIndex) {
        this.attacker = attacker;
        this.slotIndex = slotIndex;
    }

    public override NodeState Evaluate() {

        if (!attacker.CanUseSkill(slotIndex)) {
            return NodeState.FAILURE;
        }

        attacker.UseSkill(slotIndex);
        return NodeState.SUCCESS;
    }
}
