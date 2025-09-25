using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    private CharAnimationComponent _charAni;

    // 進入攻擊動畫
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_charAni == null)
            _charAni = animator.GetComponent<Player>()?.CharAnimationComponent;

        if (_charAni != null)
            _charAni.IsPlayingAttackAnimation = true;
    }

    // 動畫進行過程（0~1 normalizedTime）
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    // 離開攻擊動畫
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_charAni == null) return;

        _charAni.IsPlayingAttackAnimation = false;
    }
}
