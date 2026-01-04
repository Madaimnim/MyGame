using UnityEngine;

public class EnemyEnterExitBehaviour : StateMachineBehaviour
{
    private Enemy enemy;
    //private float enterTime; // 記錄進入動畫的時間

    // 進入攻擊動畫
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) enemy = animator.GetComponentInParent<Enemy>();

        // 鎖定，避免其他動畫
        enemy.StateComponent.SetIsPlayingAttackAnimation(true);


    }

    // 動畫進行過程（0~1 normalizedTime）
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

    }

    // 離開攻擊動畫
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

        // 解除動畫鎖定
        enemy.StateComponent.SetIsPlayingAttackAnimation ( false);
    }
}
