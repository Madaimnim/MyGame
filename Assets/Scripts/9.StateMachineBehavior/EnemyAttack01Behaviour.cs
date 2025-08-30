using UnityEngine;

public class EnemyAttack01Behaviour : StateMachineBehaviour
{
    private Enemy enemy;

    // �i�J�����ʵe
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) enemy = animator.GetComponent<Enemy>();
        Debug.Log($"{animator.gameObject.name}���ʵeState:{animator.GetCurrentAnimatorStateInfo(layerIndex)}�}�l");
        
        // ��w�����A�קK���ƥX��
        enemy.isPlayingActionAnimation=true;
    }

    // �ʵe�i��L�{�]0~1 normalizedTime�^
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;
        // �Ҧp�b�ʵe���� 30% ���ɭԥͦ��ޯ�Gif (stateInfo.normalizedTime >= 0.3f )
        //Todo Ĳ�o��H���ˮ`
    }

    // ���}�����ʵe
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

        // �Ѱ������w��
        enemy.isPlayingActionAnimation = false;
        Debug.Log($"{animator.gameObject.name}���ʵeState:{animator.GetCurrentAnimatorStateInfo(layerIndex)}����");
    }
}