using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    private CharAnimationComponent _charAni;

    // �i�J�����ʵe
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_charAni == null)
            _charAni = animator.GetComponent<Player>()?.CharAnimationComponent;

        if (_charAni != null)
            _charAni.IsPlayingAttackAnimation = true;
    }

    // �ʵe�i��L�{�]0~1 normalizedTime�^
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    // ���}�����ʵe
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_charAni == null) return;

        _charAni.IsPlayingAttackAnimation = false;
    }
}
