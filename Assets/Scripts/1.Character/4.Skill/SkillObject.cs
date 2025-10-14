using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

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
public enum HitEffectPositionType
{
    ClosestPoint,
    TargetCenter,
    TargetUpper
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
    public HitEffectPositionType HitEffectPositionType;

    // --------------------------------------移動角度限制 ----------------------------------------------------------
    [Header("角度限制設定")]
    [SerializeField] private bool useAngleLimit = false; //是否啟用角度限制
    [SerializeField, Range(1f, 180f)]
    private float maxAngle = 45f;                        //最大角度（僅在啟用時顯示）
    // ----------------------------------------------------------------------------

    [Header("**生成圖片設定**")]
    public bool canRotate = true;                                       //是否允許旋轉

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
        Debug.Log($"設定後Position{transform.position}");

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
    public Vector2 ClampDirection(Vector2 inputDir) {
        if (!useAngleLimit)
            return inputDir.normalized;

        //  根據角色面向設定基準方向
        Vector2 baseDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // 計算角度差（帶正負號，正：上方，負：下方）
        float signedAngle = Vector2.SignedAngle(baseDir, inputDir);

        // 當面向左時，SignedAngle 會反向，需要反轉角度方向
        if (transform.localScale.x < 0)
            signedAngle = -signedAngle;

        //  限制在允許角度範圍內
        float limitedAngle = Mathf.Clamp(signedAngle, -maxAngle, maxAngle);

        // 根據面向，從正確基準方向旋轉出新方向
        Quaternion rot = Quaternion.AngleAxis(
            transform.localScale.x > 0 ? limitedAngle : -limitedAngle,
            Vector3.forward
        );

        Vector2 limitedDir = (rot * baseDir).normalized;

        return limitedDir;
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
        if (useAngleLimit)
            _moveDirection = ClampDirection(_moveDirection);
        transform.position += (Vector3)(_moveDirection * MoveSpeed * Time.deltaTime);
    }
    private void SpawnAtTargetTick() {

    }
    
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


            var hitPoint=GetHitEffectPosition(collision);

            SpriteRenderer targetRenderer = collision.GetComponent<SpriteRenderer>();
            VFXManager.Instance.Play("DamageEffect01", hitPoint, targetRenderer);
        }
    }

    private void StartDestroyTimer(float delay) {
        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
        }

        _destroyCoroutine = StartCoroutine(DestroyAfterDelay(delay));
    }


    private Vector2 GetHitEffectPosition(Collider2D col) {
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
    private Vector2 GetRandomPointNear(Collider2D col, Vector2 refCenter, Vector2 extents, float sizeRatio = 0.3f) {
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

    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    #region Editor

#if UNITY_EDITOR
    // 讓 maxAngle 僅在 useAngleLimit = true 時顯示
    [UnityEditor.CustomEditor(typeof(SkillObject))]
    public class SkillObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            // 預設顯示所有欄位
            DrawPropertiesExcluding(serializedObject, new string[] { "maxAngle" });

            // 僅在 useAngleLimit = true 時顯示角度欄位
            var useAngleProp = serializedObject.FindProperty("useAngleLimit");
            if (useAngleProp.boolValue)
            {
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAngle"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    #endregion
}
