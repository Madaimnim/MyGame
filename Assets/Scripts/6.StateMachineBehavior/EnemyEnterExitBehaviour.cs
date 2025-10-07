using UnityEngine;

public class EnemyEnterExitBehaviour : StateMachineBehaviour
{
    private Enemy enemy;
    //private float enterTime; // �O���i�J�ʵe���ɶ�

    // �i�J�����ʵe
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) enemy = animator.GetComponent<Enemy>();

        // ��w�A�קK��L�ʵe
        enemy.AnimationComponent.IsPlayingAttackAnimation = true;
        

    }

    // �ʵe�i��L�{�]0~1 normalizedTime�^
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

    }

    // ���}�����ʵe
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

        // �Ѱ��ʵe��w
        enemy.AnimationComponent.IsPlayingAttackAnimation = false;

        //float exitTime = Time.time;
        //float duration = exitTime - enterTime; // ����ɶ�
        //Debug.Log($"���}{stateInfo.fullPathHash}�ʵe�AisPlayingActionAnimation�]��{enemy.isPlayingActionAnimation},�g�L�ɶ�={duration:F2}��");
    }
}
