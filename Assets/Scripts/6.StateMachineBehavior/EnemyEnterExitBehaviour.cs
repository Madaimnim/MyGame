using UnityEngine;

public class EnemyEnterExitBehaviour : StateMachineBehaviour {
    private Enemy enemy;
    private EffectComponent effect;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) {
            enemy = animator.GetComponentInParent<Enemy>();
            effect = enemy?.EffectComponent;
        }

        if (enemy == null) return;

        enemy.StateComponent.SetIsCastingSkill(true);

        //攻擊變紅（只給敵人）
        if (effect != null) {
            float duration = stateInfo.length / Mathf.Max(animator.speed, 0.0001f);
            effect.PlayAttackTint(Color.red, duration);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

        enemy.StateComponent.SetIsCastingSkill(false);

        //確保中斷也會清掉
        effect?.StopAttackTint();
    }
}
