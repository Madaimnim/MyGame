using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


public enum HitEffectPositionType
{
    ClosestPoint,
    TargetCenter,
    TargetUpper
}
public enum OnHitType
{
    [InspectorName("沒反應")] Nothing,                      //沒反應
    [InspectorName("命中消失")] Disappear,                  //命中即消失
    [InspectorName("命中後停留爆炸")] Explode,              //命中後停留爆炸
}
public enum SkillMoveType
{
    [InspectorName("原地生成")] Station,
    [InspectorName("追蹤目標")] Homing,
    [InspectorName("朝目標發射")] Toward,
    [InspectorName("直線飛行")] Straight,
    [InspectorName("生成於目標位置")] SpawnAtTarget
}

public class SkillObject : MonoBehaviour
{
    public Collider2D BottomCollider => _bottomCollider;
    [SerializeField] private Collider2D _bottomCollider;

    [Header("可否旋轉")]
    public bool canRotate = true;

    [Header("技能類型設定")]
    public HitEffectPositionType HitEffectPositionType;
    public SkillMoveType MoveType;
    private Dictionary<SkillMoveType, Action> _skillMoveTypeDtny;
    public OnHitType OnHitType;
    private Dictionary<OnHitType, Action> _onHitTypeDtny;

    [Header("基本參數")]
    public LayerMask TargetLayers;
    public float MoveSpeed = 0f;
    public Vector2 SkillOffset = Vector2.zero;
    public float DestroyDelay = 0f;
    public float OnHitDestroyDelay = 0f;

    private int _damage = 0;
    private Vector2 _knockbackForce;
    private Vector2 _moveDirection;
    private Vector2 _initialDirection;
    private Vector3 _targetPosition;
    private Transform _targetTransform;

    private Coroutine _destroyCoroutine;
    // 改用 Dictionary 來保存：目標與對應 Collider
    private readonly Dictionary<IInteractable, Collider2D> _targetDict = new Dictionary<IInteractable, Collider2D>();

    public void Initial(int power, Vector2 knockbackForce, Vector3 targetPosition, Transform targetTransform = null)
    {
        _damage = power;
        _knockbackForce = knockbackForce;
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
                break;

            case SkillMoveType.Straight:
                _moveDirection = isTargetOnLeft ? Vector2.left : Vector2.right;
                break;

            case SkillMoveType.Homing:
                break;

            case SkillMoveType.SpawnAtTarget:
                transform.position = referencePos + new Vector3(isTargetOnLeft ? -SkillOffset.x : SkillOffset.x, SkillOffset.y);
                Debug.Log($"最終Position{transform.position}");
                break;

            default:
                Debug.LogWarning($"未處理的 SkillMoveType: {MoveType}");
                break;
        }

        StartDestroyTimer(DestroyDelay);
    }
    private void InitialOnHitTypeDtny()
    {
        _onHitTypeDtny = new Dictionary<OnHitType, Action> {
            { OnHitType.Nothing, OnHitNothing},
            { OnHitType.Disappear, OnHitDisappear},
            { OnHitType.Explode, OnHitExplode}
        };
    }
    private void InitialSkillMoveTypeDtny()
    {
        _skillMoveTypeDtny = new Dictionary<SkillMoveType, Action> {
            { SkillMoveType.Station, StationTick },
            { SkillMoveType.Homing, HomingTick },
            { SkillMoveType.Toward, TowardTick },
            { SkillMoveType.Straight, StraightTick },
            { SkillMoveType.SpawnAtTarget, SpawnAtTargetTick }
        };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & TargetLayers) == 0) return;
        
        IInteractable target = collision.GetComponent<IInteractable>();
        if (target == null) return;
        if (_targetDict.ContainsKey(target)) return;

        _targetDict.Add(target, collision);  // 同時記錄對應的 Collider
    }

    private void CheckBottomCollider()
    {
        // 清除無效對象
        List<IInteractable> waitToRemove = new List<IInteractable>();

        foreach (var kvp in _targetDict)
        {
            var target = kvp.Key;
            var col = kvp.Value;
            if (target == null || col == null)
            {
                waitToRemove.Add(target);
                continue;
            }

            // 實際檢查 BottomCollider 是否相互接觸
            bool isTouch = BottomCollider.IsTouching(target.BottomCollider);
            if (isTouch)
            {
                waitToRemove.Add(target);

                //傷害判定
                var interactInfo = new InteractInfo()
                {
                    Source = transform,
                    Damage = _damage,
                    KnockbackForce = _knockbackForce,
                };
                target.Interact(interactInfo);

                //重新計算自毀計時
                StartDestroyTimer(OnHitDestroyDelay);

                //播放特效
                var hitPoint = GetHitEffectPosition(col);
                SpriteRenderer targetRenderer = col.GetComponent<SpriteRenderer>();
                VFXManager.Instance.Play("DamageEffect01", hitPoint, targetRenderer);
            }
        }

        // 移除已命中或失效對象
        foreach (var t in waitToRemove)
            _targetDict.Remove(t);

        // 觸發命中類型對應的行為
        _onHitTypeDtny[OnHitType]?.Invoke();
    }

    private Vector2 GetHitEffectPosition(Collider2D col)
    {
        Bounds b = col.bounds;
        Vector2 center = b.center;

        switch (HitEffectPositionType)
        {
            case HitEffectPositionType.ClosestPoint:
                Vector2 hitA = col.ClosestPoint(transform.position);
                Vector2 hitB = GetComponent<Collider2D>().ClosestPoint(col.transform.position);
                return (hitA + hitB) / 2f;

            case HitEffectPositionType.TargetCenter:
                return GetRandomPointNear(col, b.center, b.extents);

            case HitEffectPositionType.TargetUpper:
                // 在上緣 0.8 比例的高度作為中心
                Vector2 upperCenter = new Vector2(
                    b.center.x,
                    b.min.y + b.size.y * 0.8f
                );
                return GetRandomPointNear(col, upperCenter, b.extents);

            default:
                return b.center;
        }
    }
    private Vector2 GetRandomPointNear(Collider2D col, Vector2 refCenter, Vector2 extents, float sizeRatio = 0.3f)
    {
        Vector2 point;
        int safety = 20; // 最多嘗試20次，避免極端情況
        float rangeX = extents.x * sizeRatio;
        float rangeY = extents.y * sizeRatio;

        do
        {
            // 在正方形範圍內隨機取一點
            float x = UnityEngine.Random.Range(refCenter.x - rangeX, refCenter.x + rangeX);
            float y = UnityEngine.Random.Range(refCenter.y - rangeY, refCenter.y + rangeY);
            point = new Vector2(x, y);
            safety--;
        } while (!col.OverlapPoint(point) && safety > 0);

        return point;
    }


    //移動方法
    private void TowardTick()
    {
        transform.position += (Vector3)(_moveDirection * MoveSpeed * Time.deltaTime);
    }
    private void HomingTick()
    {
        if (_targetTransform != null)
            _moveDirection = (_targetTransform.position - transform.position).normalized;
        else
            _moveDirection = _initialDirection; // 沒目標時沿原方向飛行
        transform.position += (Vector3)(_moveDirection * MoveSpeed * Time.deltaTime);
    }
    private void StationTick()
    { }
    private void StraightTick()
    {
        transform.position += (Vector3)(_moveDirection * MoveSpeed * Time.deltaTime);
    }
    private void SpawnAtTargetTick()
    {
    }

    //碰撞方法
    private void OnHitDisappear()
    {
        StartDestroyTimer(OnHitDestroyDelay);                       //命中後重設自毀計時
    }
    private void OnHitExplode()
    {
        StartDestroyTimer(OnHitDestroyDelay);                       //命中後重設自毀計時
    }
    private void OnHitNothing()
    { }
    private void StartDestroyTimer(float delay)
    {
        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
        }

        _destroyCoroutine = StartCoroutine(DestroyAfterDelay(delay));
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void Awake()
    {
        InitialSkillMoveTypeDtny();
        InitialOnHitTypeDtny();
    }
    private void Update()
    {
        _skillMoveTypeDtny[MoveType]?.Invoke();
        CheckBottomCollider();
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (!canRotate || _moveDirection == Vector2.zero)
            return;

        float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;

        var newScale = new Vector3(1, 1, 1);

        float displayAngle = angle;
        if (angle > 90)
            displayAngle = -180 + angle;
        else if (angle < -90)                   // 左下象限 (-180°~-90°)
            displayAngle = 180f + angle;  // 例如 -135° → -45°

        transform.rotation = Quaternion.Euler(0f, 0f, displayAngle);
    }
}