using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Mono.Cecil.Cil;
public enum SkillMoveType
{
    [InspectorName("原地生成")] Station,
    [InspectorName("追蹤目標")] Homing,
    [InspectorName("朝目標發射")] Toward,
    [InspectorName("直線飛行")] Straight,
    [InspectorName("生成於目標位置")] SpawnAtTarget
}
public enum OnHitType
{
    [InspectorName("沒反應")] Nothing,                      //沒反應
    [InspectorName("命中消失")] Disappear,                  //命中即消失
    [InspectorName("命中後停留爆炸")] Explode,              //命中後停留爆炸
}

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
    public OnHitType OnHitType;
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

    private void Update() {
        _skillMoveTypeDtny[MoveType]?.Invoke();
        UpdateRotation();  // 每一幀檢查是否需要旋轉
    }


    public void Initial(float power, float knockbackPower,  Vector3 targetPosition,Transform targetTransform = null) {
        _power = power;
        _knockbackPower = knockbackPower;
        _targetPosition = targetPosition;
        _targetTransform = targetTransform;

        Vector3 referencePos = _targetTransform ? _targetTransform.position : _targetPosition;

        bool isTargetOnLeft = referencePos.x < transform.position.x;
        // 鏡像處理localScale
        transform.localScale = new Vector3(isTargetOnLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                                   transform.localScale.y,
                                   transform.localScale.z);

        transform.position = (Vector2)transform.position + new Vector2(isTargetOnLeft ? -SkillOffset.x : SkillOffset.x, SkillOffset.y);
        _initialDirection = (referencePos - transform.position).normalized;
        _moveDirection = _initialDirection;
        UpdateRotation();



       
        switch (MoveType)
        {
            case SkillMoveType.Station:
                break;

            case SkillMoveType.Toward:
                _moveDirection = (referencePos - transform.position).normalized;
                break;

            case SkillMoveType.Straight:
                _moveDirection = isTargetOnLeft ? Vector2.left : Vector2.right;
                break;

            case SkillMoveType.Homing:
                break;

            case SkillMoveType.SpawnAtTarget:
                transform.position =
                    (Vector2)referencePos
                    + new Vector2(isTargetOnLeft ? -SkillOffset.x : SkillOffset.x, SkillOffset.y)
                    - ((Vector2)spawnPivot.position - (Vector2)transform.position);
                break;

            default:
                Debug.LogWarning($"未處理的 SkillMoveType: {MoveType}");
                break;
        }

  
        StartDestroyTimer(DestroyDelay);
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
    private void InitialOnHitTypeDtny() {
        _onHitTypeDtny = new Dictionary<OnHitType,Action> {
            { OnHitType.Nothing, OnHitNothing},
            { OnHitType.Disappear, OnHitDisappear},
            { OnHitType.Explode, OnHitExplode}
        };
    }

    private void UpdateRotation() {
        if (!canRotate || _moveDirection == Vector2.zero)
            return;

        float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;

        var newScale = new Vector3(1,1,1);

        float displayAngle = angle;
        if (angle > 90)          
            displayAngle = -180+angle;   
        else if(angle < -90)                   // 左下象限 (-180°~-90°)
            displayAngle = 180f + angle;  // 例如 -135° → -45°


        transform.rotation = Quaternion.Euler(0f, 0f, displayAngle);
    }


    //移動方法
    private void StationTick() {}
    private void HomingTick() {
        if (_targetTransform != null)
            _moveDirection = (_targetTransform.position - transform.position).normalized;
        else 
            _moveDirection = _initialDirection; // 沒目標時沿原方向飛行
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


    private void OnTriggerEnter2D(Collider2D collision) {
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

            _onHitTypeDtny[OnHitType]?.Invoke();

            Vector2 hitA = collision.ClosestPoint(transform.position);
            Vector2 hitB = GetComponent<Collider2D>().ClosestPoint(collision.transform.position);
            Vector2 hitPoint = (hitA + hitB) / 2f;

            SpriteRenderer targetRenderer = collision.GetComponent<SpriteRenderer>();
            VFXManager.Instance.Play("DamageEffect01", hitPoint, targetRenderer);
        }
    }
    public void StartDestroyTimer(float delay) {
        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
        }

        _destroyCoroutine = StartCoroutine(DestroyAfterDelay(delay));
    }
    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
