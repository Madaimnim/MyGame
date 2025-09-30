using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour, IAttackable, IMoveable
{
    [Header("AI樹更新頻率")]
    public float updateInterval = 0.1f;
    private float updateTimer = 0f;
    private BehaviorTree behaviorTree;
    private Enemy enemy;

    private PendingSkillSlot currentPendingSkillSlot;
    private class PendingSkillSlot
    {
        public int slotIndex;
        public EnemySkillRuntime skillData;
        public GameObject detector;
    }


    [Header("移動停止的抓力")]
    public float stopMoveDragPower;


    public Transform currentMoveTarget { get; private set; }


    //RequestAttack使用變數
    #region



    #region 變數
    public MoveStrategyBase moveStrategy; // 存儲移動策略

    private IDamageable currentAttackDamageable;
    private Transform currentAttackTarget;
    private int currentAttackPower;
    private float currentKnockbackForce;
    private Vector3 currentKnockbackDirection;
    private GameObject currentAttackPrefab;
    #endregion

    //冷卻時間、冷卻計時器、預製體用變數
    #region   
    [HideInInspector] public float[] slotCooldowns = new float[4];      // 冷卻時間（不變動，從 SkillData 來)
    private float[] slotCooldownTimers = new float[4];                  // 冷卻計時器   
    public GameObject[] slotDetectPrefabs = new GameObject[4];          // 偵測器物件
    public string[] animationNames = new string[4];                      // 動畫名稱
    #endregion

    #endregion

    //生命週期
    #region Awake()方法
    private void Awake() {
        enemy = GetComponent<Enemy>();
        behaviorTree = GetComponent<BehaviorTree>();
    }

    private IEnumerator Start() {
        yield return new WaitUntil(() => enemy.isEnemyDataReady);

        SetCooldown();
        InitiallySetMoveStrategy();
        SetBehaviorTree(); // 設定行為樹
    }

    private void Update() {
        UpdateCooldowns();
        if (!enemy.CanRunAI) return;
        if (enemy.isPlayingActionAnimation) return;
        RunBehaviorTree(); // 執行行為樹

    }
    private void RunBehaviorTree() {
        if (updateTimer <= 0f)
        {
            behaviorTree.Tick();
            updateTimer = updateInterval;
        }
        updateTimer -= Time.deltaTime;
    }


    private void OnEnable() { }
    private void OnDisable() { }
    #endregion


    //初始化技能槽冷卻時間
    #region
    private void SetCooldown() {
        for (int slot = 1; slot <= 4; slot++)
        {
            if (slotDetectPrefabs[slot - 1] != null)
                slotCooldowns[slot - 1] = enemy.Rt.GetSkill(slot).Cooldown;
        }
    }
    #endregion

    //更新冷卻方法
    #region UpdateCooldowns()
    private void UpdateCooldowns() {
        for (int i = 0; i < 4; i++)
        {
            slotCooldownTimers[i] = Mathf.Max(0, slotCooldownTimers[i] - Time.deltaTime);
        }

    }
    #endregion

    //IAttackable需實踐方法，由Action_Attack實踐
    #region CanUseSkill(int skillSlot)、UseSkill(int skillSlot)
    public bool CanUseSkill(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= slotDetectPrefabs.Length) return false;

        GameObject detectPrefab = slotDetectPrefabs[slotIndex];
        if (detectPrefab == null) return false;

        float cooldownTimer = slotCooldownTimers[slotIndex];

        TargetDetector detector = detectPrefab.GetComponent<TargetDetector>();
        if (detector == null) return false;

        // 同步 currentAttackTarget與detector裡的target
        if (detector.hasTarget)
        {
            currentAttackTarget = detector.targetTransform;
        }
        else
        {
            currentAttackTarget = null;
            currentAttackDamageable = null; // 順便清空，避免用到舊的
        }

        return detector != null && detector.hasTarget && cooldownTimer <= 0 && !enemy.isPlayingActionAnimation;
    }

    public void UseSkill(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= slotDetectPrefabs.Length) return;

        //Todo使用技能
        currentAttackTarget = slotDetectPrefabs[slotIndex].GetComponent<TargetDetector>().targetTransform;

        RequestAttack(slotIndex + 1, currentAttackTarget, animationNames[slotIndex], currentAttackTarget.GetComponent<IDamageable>());
        //技能進入冷卻
        slotCooldownTimers[slotIndex] = slotCooldowns[slotIndex];
    }
    #endregion

    public bool CanMove() {
        return enemy.CanMove;
    }

    //請求攻擊
    #region RequestAttack(int slotID, Transform targetTransform,string animationName,IDamageable player)
    public void RequestAttack(int skillId, Transform targetTransform, string animationName, IDamageable damageable) {

        currentAttackDamageable = damageable;
        currentAttackTarget = targetTransform;
        currentAttackPower = enemy.Rt.EnemySkillPool[skillId].StatsData.Power;
        currentKnockbackForce = enemy.Rt.EnemySkillPool[skillId].StatsData.KnockbackPower;
        currentKnockbackDirection = new Vector3(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y, 0).normalized;
        currentAttackPrefab = enemy.Rt.EnemySkillPool[skillId].VisualData.Prefab;
        bool isTargetOnRight = targetTransform.position.x > transform.position.x;
        transform.localScale = new Vector3(isTargetOnRight ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                                     transform.localScale.y,
                                     transform.localScale.z);
        enemy.PlayAnimation(animationName);
    }
    #endregion

    //攻擊:Animation Event使用
    #region Attack()
    public void Attack() {
        if (currentAttackDamageable == null) return;
        var targetMonoBehaviour = currentAttackDamageable as MonoBehaviour;
        if (targetMonoBehaviour == null || !targetMonoBehaviour.gameObject.activeInHierarchy)
        {
            //目標已經死掉或被關閉，直接放棄
            currentAttackDamageable = null;
            return;
        }

        DamageInfo info = new DamageInfo()
        {
            damage = currentAttackPower,
            knockbackForce = currentKnockbackForce,
            knockbackDirection = currentKnockbackDirection
        };

        //Todo
        //currentAttackDamageable.CharHealthComponent.TakeDamage(info);
    }
    #endregion

    //Todo生成技能
    #region SpawnSkill()
    private void SpawnSkill(GameObject attackPrefab) {
        if (attackPrefab != null)
        {
            //todo
        }
    }
    #endregion

    //Move方法，由行為樹Action_Move呼叫
    #region Move()
    public void Move() {
        if (enemy.isEnemyDataReady)
        {
            StartCoroutine(MoveRoutine());
        }
        else
            Debug.Log("EnemyData尚未準備好，沒有辦法移動");
    }

    //跳躍前亂數Delay
    private IEnumerator MoveRoutine() {
        float delay = Random.Range(0f, 0.5f);
        yield return new WaitForSeconds(delay);

        enemy.PlayAnimation("Move");
    }

    #endregion

    //初始化MoveStrategy
    #region 私有SetMoveStrategy方法
    private void InitiallySetMoveStrategy() {
        switch (enemy.Rt.MoveStrategyType) // 根據 Enum 設置策略
        {
            case MoveStrategyType.Straight:
                moveStrategy = new StraightMoveStrategy();
                break;
            case MoveStrategyType.Random:
                moveStrategy = new RandomMoveStrategy();
                break;
            case MoveStrategyType.FollowPlayer:
                moveStrategy = new FollowPlayerMoveStrategy();
                break;
            default:
                Debug.LogError($"未定義的移動策略: {enemy.Rt.MoveStrategyType}");
                break;
        }
    }
    #endregion

    //改變移動策略
    #region  ChangeMoveStrategy(MoveStrategyType type) 
    public void ChangeMoveStrategy(MoveStrategyType type) {
        switch (type)
        {
            case MoveStrategyType.Straight:
                moveStrategy = new StraightMoveStrategy();
                break;
            case MoveStrategyType.Random:
                moveStrategy = new RandomMoveStrategy();
                break;
            case MoveStrategyType.FollowPlayer:
                moveStrategy = new FollowPlayerMoveStrategy();
                break;
            default:
                Debug.LogError($"未定義的移動策略: {type}");
                break;
        }
    }
    #endregion

    //初始化BehaviorTree
    #region SetBehaviorTree()
    private void SetBehaviorTree() {
        behaviorTree.SetRoot(new Selector(new List<Node>
        {
        new Action_Attack(this, 3),
        new Action_Attack(this, 2),
        new Action_Attack(this, 1),
        new Action_Attack(this, 0),
        new Action_Move(this)
        })); ;
    }
    #endregion

    //Animation Event 方法
    #region StartMoving()、StopMoving()
    public void StartMoving() {
        enemy.Rb.drag = 0;
        enemy.Rb.velocity = new Vector2(0, 0);
        Vector2 direction = moveStrategy.MoveDirection(this);
        float speed = enemy.Rt.StatsData.MoveSpeed;
        enemy.Rb.AddForce(new Vector2(direction.x * speed, direction.y * speed), ForceMode2D.Impulse);
        ClearMoveTarget();
    }
    public void StopMoving() {
        enemy.Rb.drag = stopMoveDragPower; // 設定較大的拖曳力，使角色自然減速
    }
    #endregion

    //供外部設定內部移動目標
    #region SetMoveTarget(Transform target)
    public void SetMoveTarget(Transform target) {
        currentMoveTarget = target;
    }

    public void ClearMoveTarget() {
        currentMoveTarget = null;
    }
    #endregion
}
