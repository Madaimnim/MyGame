using UnityEngine;

public class PlayerBaseAttackBehaviour : StateMachineBehaviour
{
    private Player _player;
    private StateComponent _stateComponent;

    // 進入攻擊動畫
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_player == null) _player = animator.GetComponentInParent<Player>();
        if (_stateComponent == null) _stateComponent = _player.StateComponent;

        _stateComponent.SetIsBaseAttacking(true);
    }

    // 動畫進行過程（0~1 normalizedTime）
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
    
    // 離開攻擊動畫
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _stateComponent.SetIsBaseAttacking(false);
    }
}
