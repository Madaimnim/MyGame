using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class SkillObject : MonoBehaviour
{

    private Vector2 _moveDirection;                                      //移動方向
    private Vector2 _initialDirection;
    private float _power = 0f;                                           //基礎攻擊力
    private float _knockbackPower;

    private Transform _targetTransform;                                  //目標
    private Vector3 _targetPosition;

    private Coroutine _destroyCoroutine;                                 //自我破壞協程
    private Dictionary<SkillMoveType, Action> _skillMoveTypeDtny;        //移動方法字典
    private Dictionary<OnHitType, Action> _onHitTypeDtny;                //移動方法字典
    private HashSet<IDamageable> _hitTargetsHash = new HashSet<IDamageable>();


    [Header("基本參數")]
    public SkillMoveType MoveType;                                      //移動方法
    public OnHitType _OnHitType;
    public float DestroyDelay = 0f;                                     //自毀時間
    public float OnHitDestroyDelay = 0f;                                //碰撞自毀時間
    public float MoveSpeed = 0f;                                        //移動速度
    public LayerMask TargetLayers;                                      //目標layer
    public Vector2 SkillOffset = Vector2.zero;                          //生成的偏移量


    [Header("**生成圖片設定**")]
    public bool canRotate = true;                                       //是否允許旋轉
    public Transform rotatePivot;                                       //旋轉基準點
    public Transform spawnPivot;                                        //生成時對準的基準點

    private void Awake() {
        InitialSkillMoveTypeDtny();
        InitialOnHitTypeDtny();
    }
    private void Start() {
        Initial();
        UpdateRotation(); 
        StartDestroyTimer(DestroyDelay);                         
    }

    private void Update() {
        _skillMoveTypeDtny[MoveType]?.Invoke();
        if (MoveType == SkillMoveType.Homing && canRotate) UpdateRotation();  // 每一幀檢查是否需要旋轉
    }

    // SkillMoveType、OnHit字典初始化，並綁定方法
    public enum SkillMoveType
    {
        [InspectorName("原地生成")] Station,
        [InspectorName("追蹤目標")] Homing,
        [InspectorName("朝目標發射")] Toward,
        [InspectorName("直線飛行")] Straight,
        [InspectorName("生成於目標位置")] SpawnAtTarget
    }
    private void InitialSkillMoveTypeDtny() {
        _skillMoveTypeDtny = new Dictionary<SkillMoveType, Action> {
            { SkillMoveType.Station, StationTick },
            { SkillMoveType.Homing, HomingTick },
            { SkillMoveType.Toward, TowardTick },
            { SkillMoveType.Straight, StraightTick },
            { SkillMoveType.SpawnAtTarget, SpawnAtTargetTick }
        };
    }
    public enum OnHitType
    {
        [InspectorName("沒反應")] Nothing,                      //沒反應
        [InspectorName("命中消失")] Disappear,                  //命中即消失
        [InspectorName("命中後停留爆炸")] Explode,              //命中後停留爆炸
    }
    private void InitialOnHitTypeDtny() {
        _onHitTypeDtny = new Dictionary<OnHitType,Action> {
            { OnHitType.Nothing, OnHitNothing},
            { OnHitType.Disappear, OnHitDisappear},
            { OnHitType.Explode, OnHitExplode}
        };
    }


    //調整生成位置與方向
    private void Initial() {
        if (_targetTransform == null) return;

        bool isTargetOnLeft = _targetTransform.position.x < transform.position.x;
        // 鏡像處理localScale
        transform.localScale = new Vector3(isTargetOnLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                                           transform.localScale.y,
                                           transform.localScale.z);

        // 處理Offset位置
        transform.position = (Vector2)transform.position + new Vector2(isTargetOnLeft ? -SkillOffset.x : SkillOffset.x, SkillOffset.y);
        switch (MoveType)
        {
            case SkillMoveType.Station:
                break;

            case SkillMoveType.Toward:
                _moveDirection = (_targetTransform.position - transform.position).normalized;
                break;

            case SkillMoveType.Straight:
                _moveDirection = isTargetOnLeft ? Vector2.left : Vector2.right;
                break;

            case SkillMoveType.Homing:
                break; 

            case SkillMoveType.SpawnAtTarget:
                transform.position = 
                    (Vector2)_targetPosition
                    + new Vector2(isTargetOnLeft ? -SkillOffset.x : SkillOffset.x, SkillOffset.y) 
                    - ((Vector2)spawnPivot.position - (Vector2)transform.position);
                break;

            default:
                Debug.LogWarning($"未處理的 SkillMoveType: {MoveType}");
                break;
        }
    }
    private void UpdateRotation() {
        if (!canRotate || _moveDirection == Vector2.zero) return;

        float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    //移動方法
    private void StationTick() {}
    private void HomingTick() {
        if (_targetTransform != null) _moveDirection = (_targetTransform.position - transform.position).normalized;
        transform.position += (Vector3)(_moveDirection * MoveSpeed * Time.deltaTime);
    }
    private void StraightTick() {
        transform.position += (Vector3)(_moveDirection * MoveSpeed * Time.deltaTime);
    }
    private void TowardTick() {
            transform.position += (Vector3)(_moveDirection * MoveSpeed * Time.deltaTime);
    }
    private void SpawnAtTargetTick() {}
    
    //碰撞方法
    private void OnHitDisappear() {
        StartDestroyTimer(OnHitDestroyDelay);                       //命中後重設自毀計時
    }
    private void OnHitNothing() {}
    private void OnHitExplode() {
        StartDestroyTimer(OnHitDestroyDelay);                       //命中後重設自毀計時
    }

    public void SetSkillProperties(float power,float knockbackPower,Transform targetTransform,Vector3 targetPosition) {
        _initialDirection = (targetPosition-transform.position).normalized;
        _moveDirection = _initialDirection;
        _knockbackPower = knockbackPower;
        _power = power;
        _targetTransform = targetTransform;
        _targetPosition = targetPosition;
    }

    //觸發TriggerEnter2D
    private void OnTriggerEnter2D(Collider2D collision) {
        // 使用 LayerMask 來檢查是否在攻擊目標內
        if (((1 << collision.gameObject.layer) & TargetLayers) != 0)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && !_hitTargetsHash.Contains(damageable))
            {
                int integerPower = Mathf.CeilToInt(_power);
                _hitTargetsHash.Add(damageable);
                DamageInfo info = new DamageInfo()
                {
                    Damage = integerPower,
                    KnockbackForce = _knockbackPower,
                    KnockbackDirection = _moveDirection
                };
                damageable.TakeDamage( info);
            }

            _onHitTypeDtny[_OnHitType]?.Invoke();

            Vector2 hitPoint = collision.ClosestPoint(transform.position);
            SpriteRenderer targetRenderer = collision.GetComponent<SpriteRenderer>();
            VFXManager.Instance.Play("DamageEffect01", hitPoint, targetRenderer);
        }
    }

    //自毀時間
    public void StartDestroyTimer(float delay) {
        if (_destroyCoroutine != null)
        {
            Debug.Log($"就銷毀倒數刪除");
            StopCoroutine(_destroyCoroutine);
        }

        _destroyCoroutine = StartCoroutine(DestroyAfterDelay(delay));
    }
    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
