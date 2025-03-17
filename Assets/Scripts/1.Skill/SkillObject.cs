using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillObject : MonoBehaviour
{
    #region 私有變數
    private Vector2 moveDirection;                                      //移動方向
    private float baseAttackPower = 0f;                                 //基礎攻擊力
    private Transform targetTransform;                                  //目標
    private float rotateAngle = 0f;                                   // 旋轉角度


    private Dictionary<SkillMoveType, Action> skillMoveTypeDtny;         // 移動方法字典

    private Animator ani;
    private HashSet<IDamageable> hitTargetsHash = new HashSet<IDamageable>();
    #endregion

    #region 可設置變數!
    [Header("基本參數")]
    public SkillMoveType moveType;                                      //移動方法
    public float skillDamage = 0f;                                      //技能傷害(百分比)
    public float destroyDelay = 0f;                                     //自毀時間
    public float moveSpeed = 0f;                                        //移動速度
    public LayerMask targetLayers;                                      //目標layer
    public Vector2 skillOffset = Vector2.zero;                          //生成的偏移量
    public float knockbackForce = 0f;                                   //擊退力

    [Header("行為參數")]
    public Vector2 scaleFactor = Vector2.one;                           //變形量
    public int moveCount = 0;                                           //異動次數
    public float resetDestroyDelay = 0f;                                //重設自毀時間
    public int chaseNextTargetCount = 0;                                //追擊目標次數
    public GameObject splitPrefab;                                      //分裂預製體

    [Header("觸擊效果")]
    public int multiHitCount = 0;                                       //多重傷害次數
    public float dotDamage = 0f;                                        //持續傷害
    public float dotDuration = 0f;                                      //持續傷害時間
    public float attackReduction = 0f;                                  //降傷比例
    public float attackReductionDuration = 0f;                          //降傷持續時間
    public float speedReduction = 0f;                                   //降速比例
    public float speedReductionDuration = 0f;                           //降速持續時間
    #endregion

    #region 生命週期
    private void Awake() {
        ani = GetComponent<Animator>();
        InitialSkillMoveTypeDtny();
    }
    private void Start() {
        Vector2 newOffset = Quaternion.Euler(0, 0, rotateAngle) * skillOffset;
        transform.position += (Vector3)newOffset;
        transform.rotation = Quaternion.Euler(0, 0, rotateAngle);

        Destroy(gameObject, destroyDelay);
    }

    private void Update() {
        skillMoveTypeDtny[moveType]?.Invoke();  
    }
    #endregion
    
    #region InitialMoveTypeDtny()
    public enum SkillMoveType
    {
        [InspectorName("原地")]
        Station,
        [InspectorName("追蹤")]
        Homing,
        [InspectorName("朝目標")]
        Toward,
        [InspectorName("直線")]
        Straight
    }
    private void InitialSkillMoveTypeDtny() {
        skillMoveTypeDtny = new Dictionary<SkillMoveType, Action>
        {
            { SkillMoveType.Station, () => { /*原地不动 */ } },
            { SkillMoveType.Homing, HomingMove },
            { SkillMoveType.Toward, TowardMove },
            { SkillMoveType.Straight, StraightMove },
        };
    }
    #endregion

    #region HomingMove()
    private void HomingMove() {
        if (targetTransform != null)
        {
            moveDirection = (targetTransform.position - transform.position).normalized;
            transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
    #endregion
    #region StraightMove()
    private void StraightMove() {
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
    }
    #endregion
    #region TowardMove()
    private void TowardMove() {
        if (targetTransform != null)
        {
            transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region SetSkillProperties(Vector2 moveDirection, float baseAttackPower, LayerMask targetLayers,Transform targetTransformm,float rotateAngle)
    public void SetSkillProperties(Vector2 moveDirection, float baseAttackPower, Transform targetTransform, float rotateAngle) {
        this.moveDirection = moveDirection.normalized;   
        this.baseAttackPower = baseAttackPower;
        this.targetTransform = targetTransform;
        this.rotateAngle = rotateAngle; // 存储旋转角度
    }
    #endregion

    #region OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collision) {
        // 使用 LayerMask 來檢查是否在攻擊目標內
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && !hitTargetsHash.Contains(damageable))
            {
                int finalDamage = Mathf.CeilToInt(baseAttackPower * skillDamage/100f);

                hitTargetsHash.Add(damageable);
                damageable.TakeDamage(
                    finalDamage, 
                    knockbackForce, 
                    dotDuration,
                    dotDamage, 
                    attackReduction,
                    attackReductionDuration, 
                    speedReduction, 
                    speedReductionDuration);
            }
        }
    }
    #endregion
}
