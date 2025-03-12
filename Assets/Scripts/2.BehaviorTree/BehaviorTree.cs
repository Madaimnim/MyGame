using System;
using UnityEngine;

public class BehaviorTree:MonoBehaviour
{
    #region 建構
    public BehaviorTree() {
    }
    #endregion

    #region 公開變數
    [HideInInspector] public bool isDead = false;        // 是否死亡
    [HideInInspector] public bool isHurt = false;        // 是否受過傷

    [HideInInspector] public bool canMove = true;        // 是否可以移動
    [HideInInspector] public bool isMoving = false;      // 是否正在移動
    [HideInInspector] public bool canStartMove = false;  // 是否開始移動

    [HideInInspector] public bool isInAttack01Range = false;   // 是否可以攻擊
    [HideInInspector] public bool isInAttack02Range = false;   // 是否可以攻擊
    [HideInInspector] public bool isInAttack03Range = false;   // 是否可以攻擊
    [HideInInspector] public bool isInAttack04Range = false;   // 是否可以攻擊

    [HideInInspector] public bool isAttack01CoolDown = true;   // 技能1是否冷卻完成
    [HideInInspector] public bool isAttack02CoolDown = true;   // 技能2是否冷卻完成
    [HideInInspector] public bool isAttack03CoolDown = true;   // 技能3是否冷卻完成
    [HideInInspector] public bool isAttack04CoolDown = true;   // 技能4是否冷卻完成

    [HideInInspector] public bool isCooldownComplete = true;// 是否冷卻結束

    [HideInInspector] public bool canChangeAnim = true;    //是否可換播動畫
    #endregion
    #region 公開Event Action
    public event Action Event_Attack01;
    public event Action Event_Attack02;
    public event Action Event_Attack03;
    public event Action Event_Attack04;
    public event Action Event_Move;
    public event Action Event_Idle;
    #endregion
    #region 私有變數
    private Node rootNode;
    #endregion

    #region 公開SetRoot()
    public void SetRoot(Node root) {
        rootNode = root;
    }
    #endregion
    #region 公開Tick()方法
    public void Tick() {
        rootNode?.Evaluate();
    }
    #endregion

    #region 公開方法
    public void Attack01Invoke() {
            Event_Attack01?.Invoke();
    }
    public void Attack02Invoke() {
        Event_Attack02?.Invoke();
    }
    public void Attack03Invoke() {
        Event_Attack03?.Invoke();
    }
    public void Attack04Invoke() {
        Event_Attack04?.Invoke();
    }

    public void MoveInvoke() {
        Event_Move?.Invoke();
    }
    public void IdleInvoke() {
        Event_Idle?.Invoke();
    }
    #endregion
}
