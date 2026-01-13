using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Cinemachine.DocumentationSortingAttribute;
using static UnityEditor.Experimental.GraphView.GraphView;


public class Player : MonoBehaviour, IInteractable, IVisualFacing {
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("Debug")]
    [SerializeField] private bool enableDebug = true;

    [SerializeField] private Collider2D _sprCol;
    public Collider2D SprCol => _sprCol;
    public Transform BottomTransform => transform;
    [SerializeField] private Transform _visualRootTransform;
    public Transform VisulaRootTransform=> _visualRootTransform;
    public Transform BackSpriteTransform;

    public TargetDetector MoveDetector;
    public GameObject SelectIndicator;
    public Transform SelectIndicatorParent;
    //組件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Ani;
    [HideInInspector] public SpriteRenderer Spr;
    //模組化 --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public PlayerStatsRuntime Rt { get; private set; }
    public StateComponent StateComponent { get; private set; }
    public ActionLockComponent ActionLockComponent { get; private set; }
    public HealthComponent HealthComponent { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public AnimationComponent AnimationComponent { get; private set; }
    public EffectComponent EffectComponent { get; private set; }
    public AIComponent AIComponent { get; private set; }
    public MoveComponent MoveComponent { get; private set; }
    public SkillComponent SkillComponent { get; private set; }
    public SpawnerComponent SpawnerComponent { get; private set; }
    public GrowthComponent GrowthComponent { get; private set; }
    public HeightComponent HeightComponent { get; private set; }
    public StatsComponent StatsComponent { get; private set; }
    public EquipmentComponent EquipmentComponent { get; private set; }
    public Vector2 MoveVelocity=>MoveComponent.IntentDirection * MoveComponent.MoveSpeed;

    private Vector3 _lastInteractPosition;
    private float _initialHeightY;
    private bool _isInitialized = false;
    private HitShakeVisual _hitShakeVisual;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponentInChildren<SpriteRenderer>();
        Ani = GetComponentInChildren<Animator>();
        _initialHeightY = Spr.transform.localPosition.y;
        _hitShakeVisual = GetComponentInChildren<HitShakeVisual>();
    }
    private void OnEnable()
    {
        if(PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);
        if(PlayerInputManager.Instance != null) PlayerInputManager.Instance.SelectPlayer(this);
        TryBindSkillSliderUI();
        if(_isInitialized) StateComponent.ResetState();
    }
    private void OnDisable()
    {
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Unregister(this);
    }
    private void Start()
    {
        if (GameManager.Instance != null) SelectIndicator = Instantiate(GameManager.Instance.SelectionIndicatorData.SelectionIndicatorPrefab, SelectIndicatorParent);
        SelectIndicator.SetActive(false);
    }
    private void Update()
    {
        if (SkillComponent != null) SkillComponent.Tick();
        if (ActionLockComponent != null) ActionLockComponent.Tick();

        //if (InputProvider != PlayerInputManager.Instance && AIComponent != null) AIComponent.Tick();

    }
    private void LateUpdate() {
        if(StateComponent!=null)StateComponent.DebugState();
    }


    private void FixedUpdate()
    {
        if (MoveComponent != null) MoveComponent.FixedTick();
        if (HeightComponent != null) HeightComponent.FixedTick();
        if(StateComponent!=null)
            if(!StateComponent.IsMoving && !StateComponent.IsAttackingIntent) AnimationComponent.PlayIdle();
    }
    public void Initialize(PlayerStatsRuntime stats)
    {
        Rt = stats;

        //注意依賴順序
        StatsComponent= new StatsComponent(Rt.StatsData);
        StateComponent = new StateComponent(DebugContext.Player,Rt.Id);
        EquipmentComponent = new EquipmentComponent(StatsComponent);

        ActionLockComponent = new ActionLockComponent(this,StateComponent);
        HealthComponent = new HealthComponent(Rt, StateComponent);
        AnimationComponent = new AnimationComponent(Ani, transform, Rb, StateComponent);

        HeightComponent = new HeightComponent(_sprCol.transform,StateComponent, AnimationComponent,this, StatsComponent.FinalStats);
        EffectComponent = new EffectComponent(transform, this, Spr, StateComponent);
        RespawnComponent = new RespawnComponent(this, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb, StatsComponent.FinalStats, this, MoveDetector, AnimationComponent,HeightComponent, StateComponent);
        SpawnerComponent = new SpawnerComponent();
        if (EnemyListManager.Instance.TargetList == null) Debug.Log("EnemyListManager未初始化");
        SkillComponent = new SkillComponent(StatsComponent.FinalStats, Rt.SkillSlotCount, Rt.SkillPool, AnimationComponent, StateComponent, transform, _sprCol.transform,
            EnemyListManager.Instance.TargetList,MoveComponent,HeightComponent);
        AIComponent = new AIComponent( MoveComponent, SkillComponent, transform, Rt.MoveStrategy);
        GrowthComponent = new GrowthComponent(Rt,StatsComponent);
        //額外初始化設定
        BehaviorTree behaviourTree = PlayerBehaviourTreeFactory.Create(Rt.PlayerBehaviourTreeType, this);
        AIComponent.SetBehaviorTree(behaviourTree);

        HpSlider hpSlider = GetComponentInChildren<HpSlider>();
        hpSlider.Bind(HealthComponent);

        //事件訂閱
        HealthComponent.OnDie += OnDie;
        HealthComponent.OnHpChanged += OnHpChanged;
        RespawnComponent.OnRespawn += OnRespawn;
        GrowthComponent.OnLevelUp += EffectComponent.LevelUpEffect;
        GrowthComponent.OnExpGained += EffectComponent.GainedExpEffect;
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Event_BattleStart += AIComponent.EnableAI;
            GameEventSystem.Instance.Event_OnWallBroken += AIComponent.DisableAI;
            GameEventSystem.Instance.Event_BattleStart += RespawnComponent.EnableRespawn;
            GameEventSystem.Instance.Event_OnWallBroken += RespawnComponent.DisableRespawn;
        }
        SkillComponent.OnSkillAnimationPlayed += TurnFacingByIntent;
        MoveComponent.OnMoveDirectionChanged += TurnFacingByIntent;

        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"玩家ID_{Rt.Id}:({Rt.Name})";

        TryBindSkillSliderUI();
        ResetState();
        _isInitialized = true;
    }
    public void Interact(InteractInfo info)
    {
        _lastInteractPosition = info.SourcePosition;
        var directX = info.SourcePosition.x - SprCol.transform.localPosition.x;

        EffectComponent.TakeDamageEffect(info.Damage);

        if (info.Damage<=0f) return;


        HealthComponent.TakeDamage(info.Damage);
        _hitShakeVisual.Play(HitShakeType.PushBack, directX);
        if (info.KnockbackPower != 0f) MoveComponent.Knockbacked(info.KnockbackPower, info.SourcePosition);
        MoveComponent.StopSkillDashMoveCoroutine();
        HeightComponent.StopSkillDashMoveCoroutine();
        HeightComponent.StopRecoverHeightCoroutine();

        HeightComponent.AddUpVelocity(info.FloatPower);
    }


    //事件方法
    public void OnDie()
    {

        AnimationComponent.PlayDie();
        StartCoroutine(Die());
    }
    private IEnumerator Die()
    {
        SkillComponent.ClearAllSkillIntent();
        MoveComponent.ClearAllMoveIntent();

        AIComponent.DisableAI();
        EffectComponent.PlayerDeathEffect();

        Vector2 dir = _lastInteractPosition-transform.position ;
        //SetFacingRight(dir); // 


        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        yield return null; //等一幀，讓Animator確實切到Die動畫
        AnimatorStateInfo state = Ani.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length / Ani.speed);

        RespawnComponent.RespawnAfter(3f);

        //發事件
        GameEventSystem.Instance.Event_OnPlayerDie?.Invoke(this);
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Unregister(this);
    }

    public void OnHpChanged(int currentHp, int maxHp) { }    //Todo SliderChange
    public void OnRespawn()
    {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = true;


        ResetState();

        AIComponent.EnableAI();
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);
    }
    public void ResetState()
    {
        StateComponent.ResetState();
        MoveComponent.ResetVelocity();
        HealthComponent.ResetCurrentHp();
    }

    private void TurnFacingByIntent(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var scale = VisulaRootTransform.localScale;
            float mag = Mathf.Abs(scale.x);
            scale.x = (direction.x < 0f) ? -mag : mag;          //假設朝向即向右，向左即向左
            VisulaRootTransform.localScale = scale;
        }
    }

    private void TryBindSkillSliderUI() {
        if (SkillComponent == null) return;
        if (UIManager.Instance == null) return;
        if (UIManager.Instance.UI_SkillSliderController == null) return;

        UIManager.Instance.UI_SkillSliderController
            .BindSkillComponent(SkillComponent, Rt.SkillPool);
    }
}