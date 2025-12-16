using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    private StateComponent _stateComponent;

    // 進入攻擊動畫
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_stateComponent == null)
            _stateComponent = animator.GetComponent<Player>()?.StateComponent;

        if (_stateComponent != null)
            _stateComponent.SetIsPlayingAttackAnimation (true);
    }

    // 動畫進行過程（0~1 normalizedTime）
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    // 離開攻擊動畫
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_stateComponent == null) return;

        _stateComponent.SetIsPlayingAttackAnimation(false);
    }
}
