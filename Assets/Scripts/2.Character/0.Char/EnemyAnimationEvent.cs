using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    private Enemy _enemy;
    private void Awake() {
        _enemy=GetComponentInParent<Enemy>();
        if (_enemy == null) Debug.Log("Enemy is null");
    }

    public void AnimationEvent_SpawnerSkill() {
        _enemy.SkillComponent.UseSkill();
    }
    public void AnimationEvent_MoveStart(float duration)//duration = frame數/sample率
    {
        _enemy.MoveComponent.MoveDuration(duration);
    }

    public void AnimationEvent_SkillDashPrepareStart(int skillId) {
        if (!_enemy.Rt.SkillPool.TryGetValue(skillId, out var skillRt)) return;
        _enemy.SkillComponent.SkillPrepareMove(skillRt);
    }
    public void AnimationEvent_SkillDashStart(int skillId) {
        if (!_enemy.Rt.SkillPool.TryGetValue(skillId, out var skillRt)) return;

        _enemy.SkillComponent.UseSkill();
        //Todo 將進展攻擊及生成物件分開
        _enemy.SkillComponent.SkillDashMove(skillRt);
    }
}
