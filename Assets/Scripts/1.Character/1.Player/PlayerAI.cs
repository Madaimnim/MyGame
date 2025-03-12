using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerAI : MonoBehaviour
{
    #region 公開變數
    public Player player; 
    private PlayerStateManager.PlayerStats playerState;
    public BehaviorTree behaviorTree;
    #endregion

    #region Awake()方法
    private void Awake() {
        if (player == null)
        {
            Debug.LogError("❌ [PlayerAI] Player 未設定，請在 Inspector 拖入！");
            return;
        }
    }
    #endregion

    #region OnEnable() & OnDisable()
    private void OnEnable() {
        behaviorTree.Event_Move += Move;
        behaviorTree.Event_Attack01 += Attack;
        behaviorTree.Event_Attack02 += Attack;
        behaviorTree.Event_Attack03 += Attack;
        behaviorTree.Event_Attack04 += Attack;
    }

    private void OnDisable() {
        behaviorTree.Event_Move -= Move;
        behaviorTree.Event_Attack01 -= Attack;
        behaviorTree.Event_Attack02 -= Attack;
        behaviorTree.Event_Attack03 -= Attack;
        behaviorTree.Event_Attack04 -= Attack;
    }
    #endregion

    #region AI 行為
    public void Attack() {
        if (player != null)
            player.animator.Play(Animator.StringToHash("Attack"));
        behaviorTree.canChangeAnim = false;
    }

    public void Move() {
        Debug.Log("✅ AI 控制移動");
    }
    #endregion
}
