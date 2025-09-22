using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class SkillObject : MonoBehaviour
{
    #region 私有變數
    private Vector2 moveDirection;                                      //移動方向
    private Vector2 initialDirection;
    private float baseAttackPower = 0f;                                 //基礎攻擊力
    private float rotateAngle = 0f;                                     //旋轉角度
    private Transform targetTransform;                                  //目標
    private Coroutine destroyCoroutine;                                 //自我破壞協程

    private Dictionary<SkillMoveType, Action> skillMoveTypeDtny;        //移動方法字典
    private Dictionary<OnHitType, Action> onHitTypeDtny;                //移動方法字典
    private Animator ani;
    private SpriteRenderer spriteRenderer;
    private HashSet<IDamageable> hitTargetsHash = new HashSet<IDamageable>();
    #endregion

    #region 可設置變數：

    #region 攻擊動畫類型
    [Header("攻擊動畫類型")]
    public AttackAnimationType attackAnimationType;                     //腳色攻擊動畫類型
      
    public float attackSpawnDelayTime = 0f;                             //技能生成延遲

    public enum AttackAnimationType
    {
        [InspectorName("Attack01")] Attack01,
        [InspectorName("Attack02")] Attack02,
        [InspectorName("Attack03")] Attack03
    }
    #endregion


    #region 旋轉變數
    [Header("**生成圖片設定**")]
    public bool canRotate = true;                                       //是否允許旋轉
    public Transform rotatePivot;                                       //旋轉基準點
    public Transform spawnPivot;                                        //生成時對準的基準點
    #endregion
    #region 基本變數
    [Header("基本參數")]
    public SkillMoveType moveType;                                      //移動方法
    public OnHitType onHitType;
    public float destroyDelay = 0f;                                     //自毀時間
    public float onHitDestroyDelay = 0f;                                //碰撞自毀時間
    public float moveSpeed = 0f;                                        //移動速度
    public LayerMask targetLayers;                                      //目標layer
    public Vector2 skillOffset = Vector2.zero;                          //生成的偏移量
    public float knockbackForce = 0f;                                   //擊退力

    //Todo[Header("行為參數")]

    //Todo
    //[Header("觸擊效果")]
    //public int multiHitCount = 0;                                       //多重傷害次數
    //public float dotDamage = 0f;                                        //持續傷害
    //public float dotDuration = 0f;                                      //持續傷害時間
    //public float attackReduction = 0f;                                  //降傷比例
    //public float attackReductionDuration = 0f;                          //降傷持續時間
    //public float speedReduction = 0f;                                   //降速比例
    //public float speedReductionDuration = 0f;                           //降速持續時間
    #endregion

    #endregion

    #region 生命週期
    private void Awake() {
        spriteRenderer=GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        InitialSkillMoveTypeDtny();
        InitialOnHitTypeDtny();
    }
    private void Start() {
        AdjustSkillPositionAndRotation();                               //調整技能生成位置與方向
        StartDestroyTimer(destroyDelay);                          //自毀時間初始設置  
    }

    private void Update() {
        skillMoveTypeDtny[moveType]?.Invoke();  
    }
    #endregion

    // SkillMoveType、OnHit字典初始化，並綁定方法
    #region InitialMoveTypeDtny()
    public enum SkillMoveType
    {
        [InspectorName("原地生成")] Station,
        [InspectorName("追蹤目標")] Homing,
        [InspectorName("朝目標發射")] Toward,
        [InspectorName("直線飛行")] Straight,
        [InspectorName("生成於目標位置")] SpawnAtTarget
    }

    //初始化追蹤方法的字典( SkillMoveType,綁定方法)
    private void InitialSkillMoveTypeDtny() {
        skillMoveTypeDtny = new Dictionary<SkillMoveType, Action> {
            { SkillMoveType.Station, StationMove },
            { SkillMoveType.Homing, HomingMove },
            { SkillMoveType.Toward, TowardMove },
            { SkillMoveType.Straight, StraightMove },
            { SkillMoveType.SpawnAtTarget, SpawnAtTarget }
        };
    }

    public enum OnHitType
    {
        [InspectorName("沒反應")] Nothing,                      //沒反應
        [InspectorName("命中消失")] Disappear,                  //命中即消失
        [InspectorName("命中後停留爆炸")] Explode,              //命中後停留爆炸
    }

    //初始化OnHit方法的字典( OnHitType,綁定方法)
    private void InitialOnHitTypeDtny() {
        onHitTypeDtny = new Dictionary<OnHitType,Action> {
            { OnHitType.Nothing, OnHitNothing},
            { OnHitType.Disappear, OnHitDisappear},
            { OnHitType.Explode, OnHitExplode}
        };
    }
    #endregion

    //調整生成位置與方向
    #region AdjustSkillPositionAndRotation()
    private void AdjustSkillPositionAndRotation() {
        
        if (targetTransform == null)
        {
            Debug.LogWarning("SkillObject 沒有可用的 targetTransform，跳過位置與旋轉調整。");
            return;
        }

        bool isTargetOnLeft = targetTransform != null && targetTransform.position.x < transform.position.x;
        // 鏡像處理localScale
        transform.localScale = new Vector3(isTargetOnLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                                           transform.localScale.y,
                                           transform.localScale.z);

        // 處理Offset位置
        transform.position = (Vector2)transform.position + new Vector2(isTargetOnLeft ? -skillOffset.x : skillOffset.x, skillOffset.y);

        // 應用旋轉
        if (canRotate)
            ApplyRotation(isTargetOnLeft);

        switch (moveType)
        {
            #region SkillMoveType.Station
            case SkillMoveType.Station:
                break;
            #endregion

            #region SkillMoveType.Homing
            case SkillMoveType.Toward:
                moveDirection = (targetTransform != null) ? (targetTransform.position - transform.position).normalized : Vector2.right;
                break;
            #endregion

            #region SkillMoveType.Toward
            case SkillMoveType.Straight:
                moveDirection = isTargetOnLeft ? Vector2.left : Vector2.right;
                break;
            #endregion

            #region SkillMoveType.Straight
            case SkillMoveType.Homing:
                break; // 追踪类技能在 Update 里处理移动
            #endregion

            #region SkillMoveType.SpawnAtTarget
            case SkillMoveType.SpawnAtTarget:
                transform.position = 
                    (Vector2)targetTransform.position 
                    +new Vector2(isTargetOnLeft ? -skillOffset.x : skillOffset.x, skillOffset.y) 
                    - ((Vector2)spawnPivot.position - (Vector2)transform.position);
                break;
            #endregion
            #region Default
            default:
                Debug.LogWarning($"⚠️ 未處理的 `SkillMoveType`: {moveType}");
                break;
            #endregion
        }
    }
    #endregion

    //技能物：生成方式&移動方法
    #region SkillMoveType方法

    //原地不動
    #region StationMove()
    private void StationMove() {
    }
    #endregion

    //追蹤目標
    #region HomingMove()
    private void HomingMove() {
        if (targetTransform != null)
        {
            moveDirection = (targetTransform.position - transform.position).normalized;
            transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
    #endregion

    //朝向目標
    #region StraightMove()
    private void StraightMove() {
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
    }
    #endregion

    //朝向目標
    #region TowardMove()
    private void TowardMove() {

            transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

    }
    #endregion

    //生成在目標位置
    #region SpawnAtTarget()
    private void SpawnAtTarget() {
        // 這個方法是空的，因為 `Start()` 時已經調整了位置
    }
    #endregion

    #endregion

    //碰撞方法
    #region OnHitDisappear()....

    //碰撞消失
    #region OnHitDisappear()
    private void OnHitDisappear() {
        StartDestroyTimer(onHitDestroyDelay);                       //命中後重設自毀計時
    }
    #endregion
    //碰撞無事
    #region OnHitNothing方法
    private void OnHitNothing() {
    }
    #endregion
    //碰撞爆炸
    #region OnHitExplode()
    private void OnHitExplode() {
        StartDestroyTimer(onHitDestroyDelay);                       //命中後重設自毀計時
    }
    #endregion


    #endregion

    //技能旋轉
    #region ApplyRotation(bool isTargetOnLeft)
    private void ApplyRotation(bool isTargetOnLeft) {
        // 调整角度
        float adjustedAngle = rotateAngle; // 默认右侧角度
        if (isTargetOnLeft)
        {
            //左半邊(90-180、-90-180) 需要转换为 (rotateAngle - 180°)
            if (rotateAngle > 90f || rotateAngle < -90f)
            {
                adjustedAngle = rotateAngle - 180;
            }
        }
        // 围绕 Sprite 中心旋转
        transform.RotateAround(rotatePivot.position, Vector3.forward, adjustedAngle);
    }
    #endregion

    //由PlayerSkillSpanwer呼叫設定內部參數
    #region SetSkillProperties(Vector2 directionVector, float baseAttackPower, Transform targetTransform, float rotateAngle)
    public void SetSkillProperties(Vector2 directionVector, float baseAttackPower, Transform targetTransform, float rotateAngle) {
        initialDirection = directionVector.normalized;
        moveDirection = directionVector.normalized;
        this.baseAttackPower = baseAttackPower;
        this.targetTransform = targetTransform;
        this.rotateAngle = rotateAngle;
    }
    #endregion

    //觸發TriggerEnter2D
    #region OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collision) {
        // 使用 LayerMask 來檢查是否在攻擊目標內
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && !hitTargetsHash.Contains(damageable))
            {
                int finalDamage = Mathf.CeilToInt(baseAttackPower );

                hitTargetsHash.Add(damageable);
                DamageInfo info = new DamageInfo()
                {
                    damage = finalDamage,
                    knockbackForce = knockbackForce,
                    knockbackDirection = initialDirection
                };

                damageable.TakeDamage( info);
                onHitTypeDtny[onHitType]?.Invoke();                                              //觸發OnHitTypeDtny裡的對應方法。
            }
        }
    }
    #endregion

    //自毀時間
    #region StartDestroyTimer(float delay)
    public void StartDestroyTimer(float delay) {
        if (destroyCoroutine != null)
            StopCoroutine(destroyCoroutine);
        destroyCoroutine = StartCoroutine(DestroyAfterDelay(delay));
    }

    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    #endregion
}
