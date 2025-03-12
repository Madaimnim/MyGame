using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

//SetMoveStrategy(); // 改到獨立MoveController裡去
//CoolDown方法待與BehaviorTree連動

public class EnemyAI : MonoBehaviour
{
    #region 公開變數
    public Enemy enemy;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Rigidbody2D rb;
    public MoveStrategyBase moveStrategy; // 存儲移動策略
    public SkillStrategyBase skillStrategy; // 存儲技能策略
    public EnemySkillSpawner skillSpawner;
    public BehaviorTree behaviorTree;
    public ShadowController shadowController;
    public float stopMoveDragPower;

    #endregion
    #region 私有變數


    #endregion

    #region Awake()方法
    private void Awake() {
    }
    #endregion
    #region OnEnable()
    private void OnEnable() {
        behaviorTree.Event_Move += Move;
        behaviorTree.Event_Attack01 += Attack;
    }
    #endregion
    #region OnDisable()
    private void OnDisable() {
        behaviorTree.Event_Move -= Move;
        behaviorTree.Event_Attack01 -= Attack;
    }
    #endregion
    #region Start()方法
    void Start() {
        SetMoveStrategy(); 
        SetBehaviorTree(); // 設定行為樹
    }
    #endregion
    
    #region FixUpdata
    private void FixedUpdate() {

    }
    #endregion
    #region Update
    void Update() {
        behaviorTree.Tick(); // 執行行為樹
    }
    #endregion

    #region 私有SetBehaviorTree()方法
    private void SetBehaviorTree() {
        behaviorTree.SetRoot(new Selector(new List<Node> // Selector 來處理優先級
        {
            new Sequence(new List<Node>
            {
                new Condition_Cooldown(behaviorTree),
                //new Condition_DetectGameWall(behaviorTree),
                new Action_Attack(behaviorTree) 
            }),
            new Action_Move(behaviorTree)
        })); ;
    }
    #endregion
    #region 私有SetMoveStrategy方法
    private void SetMoveStrategy() {
        //Todo switch (enemy.Stats.moveStrategyType) // 根據 Enum 設置策略
        //{
        //    case EnemyStatData.MoveStrategyType.Straight:
        //        if (GameManager.Instance.debugSettings.logStrategy)
        //            Debug.Log("選擇了Straight策略");
        //        moveStrategy = new StraightMoveStrategy();
        //        break;
        //    case EnemyStatData.MoveStrategyType.Random:
        //        if (GameManager.Instance.debugSettings.logStrategy)
        //            Debug.Log("選擇了Random策略");
        //        moveStrategy = new RandomMoveStrategy();
        //        break;
        //    case EnemyStatData.MoveStrategyType.FollowPlayer:
        //        if (GameManager.Instance.debugSettings.logStrategy)
        //            Debug.Log("選擇了FollowPlayer策略");
        //        moveStrategy = new FollowPlayerMoveStrategy();
        //        break;
        //    default:
        //        Debug.LogError($"未定義的移動策略: {enemy.Stats.moveStrategyType}");
        //        break;
        //}
    }
    #endregion

    #region 公有Attack()方法
    public void Attack() {
        animator.Play(Animator.StringToHash("Attack"));
        behaviorTree.canChangeAnim = false;
        enemy.StartCooldown();
    }

    #endregion
    #region 公有Move方法()
    public void Move() {
        if (moveStrategy != null)
        {
            animator.Play(Animator.StringToHash("Move"));
            behaviorTree.canChangeAnim = false;
        }
        else
        {
            Debug.LogError(" moveStrategy是空的");
        }
    }
    #endregion
    #region 公有AdjustShadowAlpha()方法，AnimationEvent調用
    public void AdjustShadowAlpha() {
        if (shadowController != null)
        {
            shadowController.AdjustShadowAlpha();
        }
        else
            Debug.LogError("shadowController為空");

    }
    #endregion

    #region AnimationEvent
    #region StartMoving()
    public void StartMoving() {
        rb.drag = 0;
        rb.velocity = new Vector2(0, 0);
        //Todo rb.AddForce(new Vector2(enemy.Stats.moveSpeed * moveStrategy.MoveDirection().x, enemy.Stats.moveSpeed * moveStrategy.MoveDirection().y), ForceMode2D.Impulse);
    }
    #endregion
    #region StopMoving
    public void StopMoving() {
        rb.drag = stopMoveDragPower; // 設定較大的拖曳力，使角色自然減速
    }
    #endregion
    #region ResetCanChangeAnim
    public void ResetCanChangeAnim() {
        behaviorTree.canChangeAnim = true;
    }
    #endregion
    #endregion

 
}
