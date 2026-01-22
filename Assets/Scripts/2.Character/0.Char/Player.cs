using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

//BottomTransform:角色底部位置(水平移動用)
//ScaleTransform:轉向Scale，與UIAnchor分離，避免UI轉向
//SpriteRender:Sprite，純視覺抖動用
//變形、旋轉必須同時針對Collider、Sprite、Shadow，否則回歪斜


public class Player : MonoBehaviour, IInteractable, IVisualFacing {
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("Debug")]
    [SerializeField] private bool enableDebug = true;

    [SerializeField] private Transform _scaleTransform;
    private Collider2D _groundCollider;

    [SerializeField] private float _heightRange;
    public Transform BackSpriteTransform;
    public Transform BottomTransform => transform;
    public Transform ScaleTransform => _scaleTransform;
    public Collider2D GroundCollider => _groundCollider;
    private Transform _heightTransform => _spr.transform;
    public Transform SpriteTransform => _spr.transform;
    public HeightInfo HeightInfo => new HeightInfo(_heightTransform.localPosition.y, _heightTransform.localPosition.y + _heightRange);


    public GameObject SelectIndicator;
    public Transform SelectIndicatorParent;
    //組件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Ani;
    private SpriteRenderer _spr;
    //模組化 --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public PlayerStatsRuntime Rt { get; private set; }
    public StateComponent StateComponent { get; private set; }
    public HealthComponent HealthComponent { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public AnimationComponent AnimationComponent { get; private set; }
    public EffectComponent EffectComponent { get; private set; }
    public AIComponent AIComponent { get; private set; }
    public MoveComponent MoveComponent { get; private set; }
    public CombatComponent CombatComponent { get; private set; }
    public SpawnerComponent SpawnerComponent { get; private set; }
    public GrowthComponent GrowthComponent { get; private set; }
    public HeightComponent HeightComponent { get; private set; }
    public StatsComponent StatsComponent { get; private set; }
    public EquipmentComponent EquipmentComponent { get; private set; }
    public EnergyComponent EnergyComponent { get; private set; }
    public EnergyUIController EnergyUIController { get; private set; }
    public HpSlider HpSlider { get; private set; }
    public Vector2 MoveVelocity=>MoveComponent.GetCurrentMoveVelocity();

    private Vector3 _lastInteractPosition;
    private bool _isInitialized = false;
    public HitShakeVisual HitShakeVisual;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        _spr = GetComponentInChildren<SpriteRenderer>();
        _groundCollider = GetComponentInChildren<Collider2D>();
        Ani = GetComponentInChildren<Animator>();
        HitShakeVisual = GetComponentInChildren<HitShakeVisual>();

        EnergyUIController = GetComponentInChildren<EnergyUIController>();
        HpSlider = GetComponentInChildren<HpSlider>();
    }
    private void OnEnable()
    {
        if(EnergyComponent!=null) EnergyComponent.Set(3);
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);
        if(PlayerInputManager.Instance != null) PlayerInputManager.Instance.SelectPlayer(this);
        TryBindSkillSliderUI();
        if(_isInitialized) StateComponent.ResetState();
    }
    private void OnDisable()
    {
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Unregister(this);
        ClearAllIntent();
    
        StopAllCoroutines();

    }
    private void Start()
    {
        if (GameManager.Instance != null) SelectIndicator = Instantiate(GameManager.Instance.SelectionIndicatorData.SelectionIndicatorPrefab, SelectIndicatorParent);
        SelectIndicator.SetActive(false);
    }
    private void Update()
    {
        if (CombatComponent != null) CombatComponent.Tick(EnemyListManager.Instance.TargetList);
        //if(!StateComponent.IsMoving && !StateComponent.IsDead &&!StateComponent.IsHurt&& 
        //    !StateComponent.IsCastingSkill && !StateComponent.IsBaseAttacking && StateComponent.IsGrounded)
        //    AnimationComponent.PlayIdle();
        
        if (AIComponent != null && AIComponent.CanRunAI) AIComponent.Tick();

        CombatComponent.DebugIntent(DebugContext.Player, Rt.Id);

        if (HeightComponent != null) HeightComponent.Tick();
    }
    private void LateUpdate() {
        if(StateComponent!=null)StateComponent.DebugState();
    }

    private void FixedUpdate()
    {
        if (MoveComponent != null) MoveComponent.FixedTick();

    }
    public void Initialize(PlayerStatsRuntime stats)
    {
        Rt = stats;

        //注意依賴順序
        EnergyComponent= new EnergyComponent();
        StatsComponent = new StatsComponent(Rt.StatsData);
        StateComponent = new StateComponent(this,DebugContext.Player,Rt.Id);
        EquipmentComponent = new EquipmentComponent(StatsComponent);

        HealthComponent = new HealthComponent(Rt, StateComponent);
        AnimationComponent = new AnimationComponent(Ani, transform, Rb, StateComponent);

        HeightComponent = new HeightComponent(_heightTransform, StateComponent, AnimationComponent,this, StatsComponent.FinalStats,
            ()=>GameSettingManager.Instance.PhysicConfig.GravityScale);
        EffectComponent = new EffectComponent(transform, this, _spr, StateComponent);
        RespawnComponent = new RespawnComponent(this, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb, StatsComponent.FinalStats, this, AnimationComponent,HeightComponent, StateComponent);
        SpawnerComponent = new SpawnerComponent();
        if (EnemyListManager.Instance.TargetList == null) Debug.Log("EnemyListManager未初始化");
        CombatComponent = new CombatComponent(this,StatsComponent.FinalStats, Rt.SkillSlotCount, Rt.SkillPool, AnimationComponent, StateComponent,
            transform, _heightTransform,EnemyListManager.Instance.TargetList,MoveComponent,HeightComponent,Rt.BaseAttackRuntime);
        AIComponent = new AIComponent( MoveComponent, CombatComponent, transform, Rt.MoveStrategy);
        GrowthComponent = new GrowthComponent(Rt,StatsComponent);
        //額外初始化設定
        BehaviorTree behaviourTree = PlayerBehaviourTreeFactory.Create(Rt.PlayerBehaviourTreeType, this);
        AIComponent.SetBehaviorTree(behaviourTree);

        HpSlider.Bind(HealthComponent);
        TryBindSkillSliderUI();
        TryBindEnergyUI();

        var playerSkillUseGate = new PlayerSkillUseGate(EnergyComponent);
        CombatComponent.SetSkillUseGate(playerSkillUseGate);
        CombatComponent.EquipSkillChain(new PlayerSkillChain());
        //事件訂閱
        HealthComponent.OnDie += OnDie;
        HealthComponent.OnHpChanged += OnHpChanged;
        RespawnComponent.OnRespawn += OnRespawn;
        GrowthComponent.OnLevelUp += EffectComponent.LevelUpEffect;
        GrowthComponent.OnExpGained += EffectComponent.GainedExpEffect;
        if (GameEventSystem.Instance != null)
        {
            //GameEventSystem.Instance.Event_BattleStart += AIComponent.EnableAI;
            GameEventSystem.Instance.Event_OnWallBroken += AIComponent.DisableAI;
            GameEventSystem.Instance.Event_BattleStart += RespawnComponent.EnableRespawn;
            GameEventSystem.Instance.Event_OnWallBroken += RespawnComponent.DisableRespawn;
        }
        CombatComponent.OnAttackTurn += TurnFacingByIntent;
        MoveComponent.OnMoveDirectionChanged += TurnFacingByIntent;
        CombatComponent.OnAttackHitTarget += EnergyComponent.GainEnergyFromAttack;


        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"玩家ID_{Rt.Id}:({Rt.Name})";

        ResetState();
        _isInitialized = true;
    }
    public void Interact(InteractInfo info)
    {
        _lastInteractPosition = info.SourcePosition;            
        EffectComponent.TakeDamageEffect(info.Damage);               //先顯示傷害，儘管為0也顯示

        if (info.Damage<=0f) return;
        EnterHurt(0.1f,info);
    }
    private void TryBindEnergyUI() {
        var energyUI = GetComponentInChildren<EnergyUIController>();
        if (energyUI != null) {
            //Debug.Log("成功綁定EnergyUI");
            energyUI.Bind(EnergyComponent);
        }
    }
    private void TryBindSkillSliderUI() {
        if (CombatComponent == null) return;
        if (UIManager.Instance == null) return;
        if (UIManager.Instance.UI_SkillSliderController == null) return;

        UIManager.Instance.UI_SkillSliderController.BindCombatComponent(CombatComponent, Rt.SkillPool);
    }
    private void EnterHurt(float duration, InteractInfo info) {  
        MoveComponent.StopSkillDashMoveCoroutine();

        var directX = info.SourcePosition.x - BottomTransform.localPosition.x;
        HitShakeVisual.Play(HitShakeType.PushBack, directX);

        MoveComponent.Knockbacked(info);
        HeightComponent.Hurt(duration, info.FloatPower,HurtType.Hard);
        AnimationComponent.PlayHurt();

        HealthComponent.TakeDamage(info.Damage);
    }



    private void OnDie()                        //事件方法
    {
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Unregister(this);

        ClearAllIntent();
        SetAllColliderEnable(false);

        AnimationComponent.PlayDie();
        EffectComponent.PlayerDeathEffect();
        RespawnComponent.RespawnAfter(3f);
    }


    public void OnHpChanged(int currentHp, int maxHp) { }    //Todo SliderChange
    public void OnRespawn()
    {
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);

        ResetState();
    }


    public void ResetState()
    {

        StateComponent.ResetState();
        HealthComponent.ResetCurrentHp();
        HeightComponent.ResetInitialHeight();
        SetAllColliderEnable(true);
    }
    public void ClearAllIntent() {
        MoveComponent.ClearAllMoveIntent();
        CombatComponent.ClearSkillIntent();
        CombatComponent.ClearBaseAttackTargetTransform();
        CombatComponent.ResetSkillChain();
        EnemyOutlineManager.Instance.ClearTarget();

        AIComponent.DisableAI();
        AIComponent.SetAutoMoveTargetPosition (null);
        CombatComponent.SetAllDetectRangesVisible(false);
    }
    private void SetAllColliderEnable(bool value) {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = value;
    }

    private void TurnFacingByIntent(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var scale = ScaleTransform.localScale;
            float mag = Mathf.Abs(scale.x);
            scale.x = (direction.x < 0f) ? -mag : mag;          //假設朝向即向右，向左即向左
            ScaleTransform.localScale = scale;
        }
    }

}