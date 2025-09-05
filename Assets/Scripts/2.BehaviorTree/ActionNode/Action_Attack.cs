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
        Debug.Log("進入canUseSkill判斷");
        if (!attacker.CanUseSkill(skillSlot)) {
            return NodeState.FAILURE;
        }
        Debug.Log("判斷成功進入攻擊UseSkill");
        attacker.UseSkill(skillSlot);
        return NodeState.SUCCESS;
    }
}
