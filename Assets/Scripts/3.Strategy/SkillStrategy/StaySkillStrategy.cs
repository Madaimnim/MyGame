using UnityEngine;
public class StaySkillStrategy : SkillStrategyBase
{
    public override void ExecuteAttack(GameObject attackObject, float speed) {
        // ❌ 不移動，直接留在原地
    }
}