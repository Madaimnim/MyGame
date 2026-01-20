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
        for (int skillSlotNumber = 1; skillSlotNumber <= _combatComponent.SkillSlotCount; skillSlotNumber++)
        {
            //Debug.Log($"嘗試使用技能槽 {skillSlotNumber} 的技能");

            var slot = _combatComponent.SkillSlots[skillSlotNumber-1];
            if(slot == null || !slot.IsReady) continue;
            if (slot.Detector == null || !slot.Detector.HasTarget) continue;

            //Debug.Log($"技能槽 {skillSlotNumber} 有目標，嘗試設定意圖");
            if (_combatComponent.SetIntentSkill(skillSlotNumber, slot.Detector.TargetTransform.position, slot.Detector.TargetTransform)) {
                //Debug.Log($"成功設定技能槽 {skillSlotNumber} 的攻擊意圖");
                return NodeState.SUCCESS;
            }

            else
                continue;
        }

        return NodeState.FAILURE; // 沒有任何技能能用
    }
}
