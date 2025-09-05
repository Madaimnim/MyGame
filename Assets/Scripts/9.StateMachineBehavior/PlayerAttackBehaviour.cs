using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    private Player player;

    // �i�J�����ʵe
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (player == null) player = animator.GetComponent<Player>();

        // ��w�A�קK��L�ʵe
        player.SetPlayingAttackAnimation(true);
    }

    // �ʵe�i��L�{�]0~1 normalizedTime�^
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (player == null) return;

    }

    // ���}�����ʵe
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (player == null) return;

        // �Ѱ��ʵe��w
        player.SetPlayingAttackAnimation(false);
    }
}
