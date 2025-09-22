using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    private Player player;

    // 進入攻擊動畫
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (player == null) player = animator.GetComponent<Player>();

        // 鎖定，避免其他動畫
        player.SetPlayingAttackAnimation(true);
    }

    // 動畫進行過程（0~1 normalizedTime）
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (player == null) return;

    }

    // 離開攻擊動畫
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (player == null) return;

        // 解除動畫鎖定
        player.SetPlayingAttackAnimation(false);
    }
}
