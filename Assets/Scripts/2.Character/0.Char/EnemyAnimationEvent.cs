using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyAnimationEvent : MonoBehaviour
{
    private Enemy _enemy;
    private void Awake() {
        _enemy=GetComponentInParent<Enemy>();
        if (_enemy == null) Debug.LogWarning("Enemy is Null!");
    }

    public void AnimationEvent_SpawnerSkill() {
        _enemy.CombatComponent.SpawnSkill();
    }
    public void AnimationEvent_MoveStart(float duration)//duration = frame¼Æ/sample²v
    {
        _enemy.MoveComponent.MoveDuration(duration);
    }

    public void AnimationEvent_RecoverHeight() {
        var _stateComponent = _enemy.StateComponent;
        if (_stateComponent.IsGrounded) {
            _stateComponent.SetIsRecoveringHeight(true);
            _stateComponent.SetIsAntiGravity(true);
        }
    }


    public void AnimationEvent_SkillDashPrepareStart(int skillId) {
        if (!_enemy.Rt.SkillPool.TryGetValue(skillId, out var skillRt)) return;
        _enemy.CombatComponent.SkillPrepareMove(skillRt);
    }
    public void AnimationEvent_SkillDashStart(int skillId) {
        if (!_enemy.Rt.SkillPool.TryGetValue(skillId, out var skillRt)) return;

        _enemy.CombatComponent.SpawnSkill();

        _enemy.CombatComponent.SkillDashMove(skillRt);
    }
}
