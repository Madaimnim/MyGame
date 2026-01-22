using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWall : MonoBehaviour, IInteractable, IHealthData, IVisualFacing {

    [SerializeField] private Transform _scaleTransform; 
    [SerializeField] private Collider2D _groundCollider;

    [SerializeField] private float _heightRange;
    [SerializeField] private int _maxHp = 10;


    public Transform BottomTransform => transform;
    public Transform ScaleTransform => _scaleTransform;
    public Collider2D GroundCollider => _groundCollider;
    private Transform _heightTransform => _spriteRenderer.transform;
    public Transform SpriteTransform => _spriteRenderer.transform;
    public HeightInfo HeightInfo => new HeightInfo(_heightTransform.localPosition.y, _heightTransform.localPosition.y + _heightRange);


    public int MaxHp =>_maxHp;
    public int CurrentHp { get; set; }
    public HpSlider HpSlider { get; set; }
    public Vector2 MoveVelocity => Vector2.zero;


    private SpriteRenderer _spriteRenderer;
    private HitShakeVisual _hitShakeVisual ;
    private HealthComponent _healthComponent;
    private EffectComponent _effectComponent;
    private StateComponent _stateComponent ;

    private void Awake() {
        CurrentHp = MaxHp;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _hitShakeVisual = GetComponentInChildren<HitShakeVisual>();
        _stateComponent = new StateComponent(this,DebugContext.None, -1);
        _healthComponent = new HealthComponent(this, _stateComponent);
        _effectComponent = new EffectComponent(transform, this, _spriteRenderer, _stateComponent);

        HpSlider = GetComponentInChildren<HpSlider>();
        if (HpSlider == null) Debug.Log("GameWall 缺少 HpSlider 元件！");
        else HpSlider.Bind(_healthComponent);

        HpSlider.gameObject.SetActive(false);

        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);
        _healthComponent.ForceNotify();
    }
    public void OnEnable() {

        _healthComponent.OnDie += OnDie;
    }
    public void OnDisable() {
        _healthComponent.OnDie -= OnDie;
    }

    private IEnumerator Start() {
        if (HpSlider == null) yield break;

        while (!GameManager.Instance.GameStageSystem.IsBattleStarted) {
            yield return null;
        }

        HpSlider.gameObject.SetActive(true);
    }


    public void Interact(InteractInfo info) {

        _effectComponent.TakeDamageEffect(info.Damage);

        if (info.Damage <= 0f) return;

        _healthComponent.TakeDamage(info.Damage);
        _hitShakeVisual.Play(HitShakeType.PushBack, 0f);

    }

    public void OnDie() {
        GameEventSystem.Instance?.Event_OnWallBroken?.Invoke();
    }

}