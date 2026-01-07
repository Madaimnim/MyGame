using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillObject : MonoBehaviour, IInteractable {
    [SerializeField] private Collider2D _sprCol;
    public Collider2D SprCol => _sprCol;
    public Transform BottomTransform => transform;
    public Transform CharSprTransform { get; private set; }
    public bool CanGravityFall =false;

    public Vector2 MoveVelocity => _skillMoveComponent.MoveDirection * _skillMoveComponent.MoveSpeed;
    private bool _canRotate = false;
    private StatsData _pStatsData;
    private ISkillRuntime _skillRt;
    private SkillLifetimeType _skillLifetimeType;
    private SkillMoveType _skillMoveType;
    private SkillMoveComponent _skillMoveComponent;
    private SkillHitComponent _skillHitComponent;
    private SkillHeightComponent _skillheightComponent;

    private Coroutine _destroyCoroutine;

    private void Awake() {
        _skillMoveComponent = new SkillMoveComponent(this);
        _skillHitComponent = new SkillHitComponent(this);
        _skillheightComponent = new SkillHeightComponent(_sprCol.transform, CanGravityFall);
    }
    private void Update() {
        if (_skillMoveComponent != null) _skillMoveComponent.Tick();
        if (_skillHitComponent != null) _skillHitComponent.Tick();
        //UpdateRotation();
    }
    private void FixedUpdate() {
        if (_skillheightComponent == null) return;
        if(_skillMoveType== SkillMoveType.AttackToOwner) {
            _skillheightComponent.UpdateHeight(CharSprTransform.localPosition.y);
        } else _skillheightComponent.FixedTick();


    }

    public void Initial(Transform charTransform,Transform charSprTransform, StatsData pStatsData, ISkillRuntime skillRt,  Vector3 targetPosition, Transform targetTransform = null) {
        CharSprTransform = charSprTransform;
        _pStatsData = pStatsData;
        _skillRt = skillRt;
        _skillMoveType= skillRt.SkillMoveType;
        _skillLifetimeType= skillRt.SkillLifetimeType;
        _canRotate = _skillRt.canRotate;

        //模組初始化
        _skillMoveComponent.Initialize(_skillRt,charTransform, charSprTransform ,targetPosition, targetTransform);
        _skillHitComponent.Initialize(_skillRt,_skillMoveComponent, _pStatsData);


        if(_skillMoveType != SkillMoveType.ParabolaToward) UpdateRotation();
        switch (_skillLifetimeType) {
            case SkillLifetimeType.TimeLimit:
                StartDestroyTimer(_skillRt.DestroyDelay);
                break;

            case SkillLifetimeType.AnimationOnce:
                // 什麼都不做，等動畫事件
                break;
        }
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
    private void UpdateRotation() {
        if (!_canRotate || _skillMoveComponent.MoveDirection == Vector2.zero) return;
        //Debug.Log($"_canRotate:{_canRotate},MoveDirection:{_skillMoveComponent.MoveDirection}");
        float angle = Mathf.Atan2(_skillMoveComponent.MoveDirection.y, _skillMoveComponent.MoveDirection.x) * Mathf.Rad2Deg;

        var newScale = new Vector3(1, 1, 1);

        float displayAngle = angle;
        if (angle > 90)
            displayAngle = -180 + angle;
        else if (angle < -90)                   // 左下象限 (-180°~-90°)
            displayAngle = 180f + angle;  // 例如 -135° → -45°

        //_sprCol.transform.RotateAround(_sprCol.bounds.center, Vector3.forward, displayAngle - _sprCol.transform.eulerAngles.z);
        _sprCol.transform.rotation = Quaternion.Euler(0f, 0f, displayAngle);
    }
    //Todo
    public void Interact(InteractInfo info) {
        // SkillObject 待實現
    }

    //動畫事件 搭配SkillLifetimeType
    public void AnimationEvent_Destroy() { 
        Destroy(gameObject);
    }   

}