using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWall : MonoBehaviour, IInteractable, IHealthData, IVisualFacing {
    [SerializeField] private Collider2D sprCol;
    public Collider2D SprCol => sprCol;
    public Transform BottomTransform => transform;
    [SerializeField]private int _maxHp = 10;
    public int MaxHp =>_maxHp;
    public int CurrentHp { get; set; }
    public Vector2 MoveVelocity { get; }
    [SerializeField] private Transform _visualRootTransform;
    public Transform VisulaRootTransform => _visualRootTransform;

    private SpriteRenderer _spriteRenderer;

    private HitShakeVisual _hitShakeVisual ;
    private HealthComponent _healthComponent;
    private EffectComponent _effectComponent;
    private StateComponent _stateComponent ;

    private void Awake() {
        CurrentHp = MaxHp;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _hitShakeVisual = GetComponentInChildren<HitShakeVisual>();
        _stateComponent = new StateComponent();
        _healthComponent = new HealthComponent(this, _stateComponent);
        _effectComponent = new EffectComponent(transform, this, _spriteRenderer, _stateComponent);

        HpSlider hpSlider = GetComponent<HpSlider>();
        if (hpSlider == null) Debug.Log("GameWall 缺少 HpSlider 元件！");
        else hpSlider.Bind(_healthComponent);

        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);
    }
    public void OnEnable() {
        _healthComponent.OnDie += OnDie;
    }
    public void OnDisable() {
        _healthComponent.OnDie -= OnDie;
    }

    private void Start() {

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