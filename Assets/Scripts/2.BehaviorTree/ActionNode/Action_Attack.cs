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
        Debug.Log("�i�JcanUseSkill�P�_");
        if (!attacker.CanUseSkill(skillSlot)) {
            return NodeState.FAILURE;
        }
        Debug.Log("�P�_���\�i�J����UseSkill");
        attacker.UseSkill(skillSlot);
        return NodeState.SUCCESS;
    }
}
