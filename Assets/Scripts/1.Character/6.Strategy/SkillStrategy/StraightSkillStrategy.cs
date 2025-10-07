using UnityEngine;

public class StraightSkillStrategy : SkillStrategyBase
{
    public override void ExecuteAttack(GameObject attackObject, float speed) {
        Rigidbody2D rb = attackObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(speed, 0); // 向右移動
        }
    }
}