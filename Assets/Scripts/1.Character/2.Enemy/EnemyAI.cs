using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

//SetMoveStrategy(); // 改到獨立MoveController裡去
//CoolDown方法待與BehaviorTree連動

public class EnemyAI : MonoBehaviour, IAttackable
{
    //變數
    #region 變數
    public Enemy enemy;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Rigidbody2D rb;
    public MoveStrategyBase moveStrategy; // 存儲移動策略
    public SkillStrategyBase skillStrategy; // 存儲技能策略
    public List<EnemySkillSlot> enemySkillSlotList;
    public BehaviorTree behaviorTree;
    public ShadowController shadowController;
    public float stopMoveDragPower;

    public float skillSlot1CooldownTime;
    public float skillSlot2CooldownTime;
    public float skillSlot3CooldownTime;
    public float skillSlot4CooldownTime;

    public GameObject skillSlot1DetectPrefab;
    public GameObject skillSlot2DetectPrefab;
    public GameObject skillSlot3DetectPrefab;
    public GameObject skillSlot4DetectPrefab;



    #endregion
    
    //生命週期
    #region Awake()方法
    private void Awake() {}

    void Start() {
        SetMoveStrategy(); 
        SetBehaviorTree(); // 設定行為樹
    }

    void Update() {
        behaviorTree.Tick(); // 執行行為樹
    }
    #endregion

    //設置行為樹
    #region 私有SetBehaviorTree()方法
    private void SetBehaviorTree() {
        behaviorTree.SetRoot(new Selector(new List<Node> // Selector 來處理優先級
        {
        new Action_Attack(this, 4),
        new Action_Attack(this, 3),
        new Action_Attack(this, 2),
        new Action_Attack(this, 1),
        new Action_Move()
        })); ;
    }
    #endregion


    #region CanUseSkillSlot(int skillSlot)          bool 
    public bool CanUseSkill(int skillSlot) {
        switch (skillSlot)
        {
            case 1: return CanUseSkillSlot1();
            case 2: return CanUseSkillSlot2();
            case 3: return CanUseSkillSlot3();
            case 4: return CanUseSkillSlot4();
            default: return false;
        }
    }

    public bool CanUseSkillSlot1() {
        if (skillSlot1DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }

    public bool CanUseSkillSlot2() {
        if (skillSlot2DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }
    public bool CanUseSkillSlot3() {
        if (skillSlot3DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }
    public bool CanUseSkillSlot4() {
        if (skillSlot4DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }
    #endregion

    #region UseSkillSlot(int skillSlot)             void
    public void UseSkill(int skillSlot) {
        switch (skillSlot)
        {
            case 1: UseSkillSlot1(); break;
            case 2: UseSkillSlot2(); break;
            case 3: UseSkillSlot3(); break;
            case 4: UseSkillSlot4(); break;
        }
    }

    public void UseSkillSlot1() { } //skillSlot1CooldownTime = enemyStats.GetSkillAtSkillSlot(0).cooldownTime; Attack(); }
    public void UseSkillSlot2() { }//skillSlot2CooldownTime = playerStats.GetSkillAtSkillSlot(1).cooldownTime; Attack(); }
    public void UseSkillSlot3() { }//skillSlot3CooldownTime = playerStats.GetSkillAtSkillSlot(2).cooldownTime; Attack(); }
    public void UseSkillSlot4() { }//skillSlot4CooldownTime = playerStats.GetSkillAtSkillSlot(3).cooldownTime; Attack(); }
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

    #region 公有Move方法()
    public void Move() {
        if (moveStrategy != null)
        {
            animator.Play(Animator.StringToHash("Move"));
            //behaviorTree.canChangeAnim = false;
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
        //behaviorTree.canChangeAnim = true;
    }
    #endregion
    #endregion

 
}
