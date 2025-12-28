//Todo ParabolaToward movement
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour, IInteractable {
    [SerializeField] private Collider2D sprCol;
    public Collider2D SprCol => sprCol;
    public Transform BottomTransform => transform;

    public Vector2 MoveVelocity => _skillMoveComponent.MoveDirection * _skillMoveComponent.MoveSpeed;
    private bool _canRotate = false;
    private StatsData _pStatsData;
    private ISkillRuntime _skillRt;
    private SkillMoveComponent _skillMoveComponent;
    private SkillHitComponent _skillHitComponent;
    private HeightComponent _heightComponent;

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
    private void FixedUpdate() {
        if (_heightComponent != null) _heightComponent.FixedTick();
    }

    public void Initial(Transform charTransform,Transform charSprTransform, StatsData pStatsData, ISkillRuntime skillRt,  Vector3 targetPosition, Transform targetTransform = null) {
        _pStatsData = pStatsData;
        _skillRt = skillRt;
        _canRotate= _skillRt.canRotate;

        //模組初始化
        _skillMoveComponent.Initialize(_skillRt,charTransform, charSprTransform ,targetPosition, targetTransform);
        _skillHitComponent.Initialize(_skillRt,_skillMoveComponent, _pStatsData);

        //Vector3 referencePos = targetTransform ? targetTransform.position : targetPosition;

        //InitialOffset(referencePos);
        UpdateRotation();
        StartDestroyTimer(skillRt.DestroyDelay);
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

    //private void InitialOffset(Vector3 targetPosition) => transform.position = (Vector2)transform.position + new Vector2((targetPosition.x - transform.position.x >= 0) ? SkillOffset.x : -SkillOffset.x, SkillOffset.y);
    private void UpdateRotation() {
        if (!_canRotate || _skillMoveComponent.MoveDirection == Vector2.zero)
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