//Todo ParabolaToward movement
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkillMoveType {
    [InspectorName("原地生成")] Station,
    [InspectorName("追蹤目標")] Homing,
    [InspectorName("直線朝目標發射")] Toward,
    [InspectorName("拋物線朝目標發射")] ParabolaToward,
    [InspectorName("直線飛行")] Straight,
    [InspectorName("生成於目標位置")] SpawnAtTarget,
    [InspectorName("附在角色")] AttackToOwner
}
public enum OnHitType {
    [InspectorName("沒反應")] Nothing,                      //沒反應
    [InspectorName("命中消失")] Disappear,                  //命中即消失
    [InspectorName("命中後停留爆炸")] Explode,              //命中後停留爆炸
}
public enum HitEffectPositionType {
    ClosestPoint,
    TargetCenter,
    TargetUpper
}
public enum FacingDirection {
    Left,
    Right
}

public class SkillObject : MonoBehaviour, IInteractable {
    [SerializeField] private Collider2D sprCol;
    public Collider2D SprCol => sprCol;
    public Transform BottomTransform => transform;
    public FacingDirection FacingDirection = FacingDirection.Right;
    [Header("可否旋轉")]
    public bool canRotate = true;

    [Header("技能類型設定")]
    public SkillMoveType MoveType;
    public OnHitType OnHitType;
    public HitEffectPositionType HitEffectPositionType;

    [Header("基本參數")]
    public LayerMask TargetLayers;
    public float MoveSpeed = 0f;
    public Vector2 SkillOffset = Vector2.zero;
    public float DestroyDelay = 0f;
    public float OnHitDestroyDelay = 0f;
    // 碰撞用底部Y座標
    public Vector2 MoveVelocity => _skillMoveComponent.MoveDirection * _skillMoveComponent.MoveSpeed;

    private StatsData _pStatsData;
    private StatsData _sStatsData;
    private SkillMoveComponent _skillMoveComponent;
    private SkillHitComponent _skillHitComponent;

    private Coroutine _destroyCoroutine;

    private void Awake() {
        _skillMoveComponent = new SkillMoveComponent(this);
        _skillHitComponent = new SkillHitComponent(this);
    }
    private void Update() {
        if (_skillMoveComponent != null) _skillMoveComponent.Tick();
        if (_skillHitComponent != null) _skillHitComponent.Tick();
        UpdateRotation();
    }

    public void Initial(Transform charTransform,StatsData pStatsData, StatsData sStatsData, Vector3 targetPosition, Transform targetTransform = null) {
        _pStatsData = pStatsData;
        _sStatsData = sStatsData;
        //模組初始化
        _skillMoveComponent.Initialize(MoveType, MoveSpeed, charTransform ,targetPosition, targetTransform);
        _skillHitComponent.Initialize(_skillMoveComponent, _pStatsData, _sStatsData);


        Vector3 referencePos = targetTransform ? targetTransform.position : targetPosition;
        InitialOffset(referencePos);
        UpdateRotation();
        StartDestroyTimer(DestroyDelay);
    }

    private void OnTriggerEnter2D(Collider2D collision) => _skillHitComponent.TriggerEnter(collision);

    public void StartDestroyTimer(float delay) {
        if (_destroyCoroutine != null) {
            StopCoroutine(_destroyCoroutine);
        }

        _destroyCoroutine = StartCoroutine(DestroyAfterDelay(delay));
    }
    private IEnumerator DestroyAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void InitialOffset(Vector3 targetPosition) => transform.position = (Vector2)transform.position + new Vector2((targetPosition.x - transform.position.x >= 0) ? SkillOffset.x : -SkillOffset.x, SkillOffset.y);
    private void UpdateRotation() {
        if (!canRotate || _skillMoveComponent.MoveDirection == Vector2.zero)
            return;

        float angle = Mathf.Atan2(_skillMoveComponent.MoveDirection.y, _skillMoveComponent.MoveDirection.x) * Mathf.Rad2Deg;

        var newScale = new Vector3(1, 1, 1);

        float displayAngle = angle;
        if (angle > 90)
            displayAngle = -180 + angle;
        else if (angle < -90)                   // 左下象限 (-180°~-90°)
            displayAngle = 180f + angle;  // 例如 -135° → -45°

        sprCol.transform.RotateAround(sprCol.bounds.center, Vector3.forward, displayAngle - sprCol.transform.eulerAngles.z);
        //sprCol.transform.rotation = Quaternion.Euler(0f, 0f, displayAngle);
    }
    //Todo
    public void Interact(InteractInfo info) {
        // SkillObject 待實現
    }
}