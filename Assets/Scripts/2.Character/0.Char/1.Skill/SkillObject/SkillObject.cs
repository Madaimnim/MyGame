using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//BottomTransform:角色底部位置(水平移動用)
//ScaleTransform:轉向Scale，與UIAnchor分離，避免UI轉向
//SpriteRender:Sprite，純視覺抖動用
//變形、旋轉必須同時針對Collider、Sprite、Shadow，否則回歪斜

public class SkillObject : MonoBehaviour, IInteractable {

    [SerializeField] private Transform _scaleTransform;

    [SerializeField] private float _heightRange;

    public Transform BottomTransform => transform;
    public Transform ScaleTransform => _scaleTransform;
    public Collider2D GroundCollider => _groundCollider;
    private Transform _heightTransform => _spr.transform;
    public Transform SpriteTransform => _spr.transform;
    public HeightInfo HeightInfo => new HeightInfo(_heightTransform.localPosition.y, _heightTransform.localPosition.y + _heightRange);

    public Transform SourceCharTransform { get; private set; }
    public Transform SourceCharHeightTransform { get; private set; }
    
    public bool CanGravityFall =false;
    public Vector2 MoveVelocity => _skillMoveComponent.HorizontalMoveDirection * _skillMoveComponent.MoveSpeed;
    private bool _canRotate = false;
    private StatsData _pStatsData;
    private ISkillRuntime _skillRt;
    private SkillLifetimeType _skillLifetimeType;
    private SkillMoveType _skillMoveType;
    private SkillMoveComponent _skillMoveComponent;
    private SkillHitComponent _skillHitComponent;
    private SkillHeightComponent _skillheightComponent;

    private Collider2D _groundCollider;
    private SpriteRenderer _spr;
    private Coroutine _destroyCoroutine;
    //事件:命中目標，從SkillHitComponent往外接
    public event Action<ISkillRuntime> OnHitTarget;

    private void Awake() {
        _spr=GetComponentInChildren<SpriteRenderer>();
        _groundCollider=GetComponentInChildren<Collider2D>();
        _skillMoveComponent = new SkillMoveComponent(this);
        _skillHitComponent = new SkillHitComponent(this);
        _skillHitComponent.OnHitTarget += skill => {OnHitTarget?.Invoke(skill);};//事件傳遞

        _skillheightComponent = new SkillHeightComponent(_heightTransform, CanGravityFall);
    }
    private void Update() {
        if (_skillMoveComponent != null) _skillMoveComponent.Tick();
        if (_skillHitComponent != null) _skillHitComponent.Tick();
        //UpdateRotation();
    }
    private void FixedUpdate() {
        if (_skillheightComponent == null) return;
        if(_skillMoveType== SkillMoveType.AttachToOwner) {
            _skillheightComponent.UpdateHeight(SourceCharHeightTransform.localPosition.y);
        } else _skillheightComponent.FixedTick();


    }

    public void Initial(Transform charTransform,Transform charHeightTransform, StatsData pStatsData, ISkillRuntime skillRt, 
                        Vector2 initialDirection,Vector2 targetPosition, Transform targetTransform = null) {
        SourceCharTransform = charTransform;
        SourceCharHeightTransform = charHeightTransform;
        _pStatsData = pStatsData;
        _skillRt = skillRt;
        _skillMoveType= skillRt.SkillMoveType;
        _skillLifetimeType= skillRt.SkillLifetimeType;
        _canRotate = _skillRt.canRotate;

        //模組初始化
        _skillMoveComponent.Initialize(_skillheightComponent, _skillRt, SourceCharTransform, SourceCharHeightTransform, initialDirection,targetPosition, targetTransform);
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
        if (!_canRotate || _skillMoveComponent.HorizontalMoveDirection == Vector2.zero) return;
        //Debug.Log($"_canRotate:{_canRotate},MoveDirection:{_skillMoveComponent.MoveDirection}");
        float angle = Mathf.Atan2(_skillMoveComponent.HorizontalMoveDirection.y, _skillMoveComponent.HorizontalMoveDirection.x) * Mathf.Rad2Deg;

        var newScale = new Vector3(1, 1, 1);

        float displayAngle = angle;
        if (angle > 90)
            displayAngle = -180 + angle;
        else if (angle < -90)                   // 左下象限 (-180°~-90°)
            displayAngle = 180f + angle;  // 例如 -135° → -45°
        
        //_groundCollider.transform.RotateAround(_groundCollider.bounds.center, Vector3.forward, displayAngle - _groundCollider.transform.eulerAngles.z);
        _groundCollider.transform.rotation = Quaternion.Euler(0f, 0f, displayAngle);
        _heightTransform.transform.rotation = Quaternion.Euler(0f, 0f, displayAngle);
    }
    public void UpdateArrowRotation(float VerticalSpeed,float MoveSpeed) {
        float angle = Mathf.Atan2(VerticalSpeed, MoveSpeed) * Mathf.Rad2Deg;
        GroundCollider.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        _heightTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
    //Todo
    public void Interact(InteractInfo info) {
        // SkillObject 待實現
    }
}