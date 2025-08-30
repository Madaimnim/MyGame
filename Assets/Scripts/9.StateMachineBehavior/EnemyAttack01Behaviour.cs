using UnityEngine;

public class EnemyAttack01Behaviour : StateMachineBehaviour
{
    private Enemy enemy;

    // 進入攻擊動畫
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) enemy = animator.GetComponent<Enemy>();
        Debug.Log($"{animator.gameObject.name}的動畫State:{animator.GetCurrentAnimatorStateInfo(layerIndex)}開始");
        
        // 鎖定攻擊，避免重複出招
        enemy.isPlayingActionAnimation=true;
    }

    // 動畫進行過程（0~1 normalizedTime）
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;
        // 例如在動畫播放 30% 的時候生成技能：if (stateInfo.normalizedTime >= 0.3f )
        //Todo 觸發對象受傷害
    }

    // 離開攻擊動畫
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

        // 解除攻擊硬直
        enemy.isPlayingActionAnimation = false;
        Debug.Log($"{animator.gameObject.name}的動畫State:{animator.GetCurrentAnimatorStateInfo(layerIndex)}結束");
    }
}