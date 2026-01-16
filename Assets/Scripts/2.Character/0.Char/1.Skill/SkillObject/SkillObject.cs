using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//BottomTransform:角色底部位置(水平移動用)
//VisulaRootTransform:轉向Scale
//RootSpriteCollider:RootSprite，Collider、Sprite的「高度」移動用、旋轉
//SpriteRender:Sprite，純視覺抖動用

public class SkillObject : MonoBehaviour, IInteractable {
    [SerializeField] private Collider2D _rootSpriteCollider;
    public Collider2D RootSpriteCollider => _rootSpriteCollider;
    public Transform BottomTransform => transform;
    [SerializeField] private Transform _visualRootTransform;
    public Transform VisualRootTransform => _visualRootTransform;
    public Transform CharSprTransform { get; private set; }
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

    private Coroutine _destroyCoroutine;
    //事件:命中目標，從SkillHitComponent往外接
    public event Action<ISkillRuntime> OnHitTarget;

    private void Awake() {
        _skillMoveComponent = new SkillMoveComponent(this);
        _skillHitComponent = new SkillHitComponent(this);
        _skillHitComponent.OnHitTarget += skill => {OnHitTarget?.Invoke(skill);};//事件傳遞

        _skillheightComponent = new SkillHeightComponent(_rootSpriteCollider.transform, CanGravityFall);
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

    public void Initial(Transform charTransform,Transform charSprTransform, StatsData pStatsData, ISkillRuntime skillRt, 
                        Vector2 initialDirection,Vector2 targetPosition, Transform targetTransform = null) {
        CharSprTransform = charSprTransform;
        _pStatsData = pStatsData;
        _skillRt = skillRt;
        _skillMoveType= skillRt.SkillMoveType;
        _skillLifetimeType= skillRt.SkillLifetimeType;
        _canRotate = _skillRt.canRotate;

        //模組初始化
        _skillMoveComponent.Initialize(_skillRt,charTransform, charSprTransform , initialDirection,targetPosition, targetTransform);
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
        
        //_rootSpriteCollider.transform.RotateAround(_rootSpriteCollider.bounds.center, Vector3.forward, displayAngle - _rootSpriteCollider.transform.eulerAngles.z);
        _rootSpriteCollider.transform.rotation = Quaternion.Euler(0f, 0f, displayAngle);
    }

    //Todo
    public void Interact(InteractInfo info) {
        // SkillObject 待實現
    }
}