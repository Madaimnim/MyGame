using UnityEngine;

public class EnemyEnterExitBehaviour : StateMachineBehaviour
{
    private Enemy enemy;
    //private float enterTime; // 記錄進入動畫的時間

    // 進入攻擊動畫
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) enemy = animator.GetComponent<Enemy>();

        // 鎖定，避免其他動畫
        enemy.isPlayingActionAnimation = true;
        
        //enterTime = Time.time;
        //Debug.Log($"進入{stateInfo.fullPathHash}動畫，isPlayingActionAnimation設為{enemy.isPlayingActionAnimation}");
    }

    // 動畫進行過程（0~1 normalizedTime）
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

    }

    // 離開攻擊動畫
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (enemy == null) return;

        // 解除動畫鎖定
        enemy.isPlayingActionAnimation = false;

        //float exitTime = Time.time;
        //float duration = exitTime - enterTime; // 播放時間
        //Debug.Log($"離開{stateInfo.fullPathHash}動畫，isPlayingActionAnimation設為{enemy.isPlayingActionAnimation},經過時間={duration:F2}秒");
    }
}
