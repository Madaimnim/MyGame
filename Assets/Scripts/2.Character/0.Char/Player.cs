using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Player : MonoBehaviour, IInteractable
{
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("Debug")]
    [SerializeField] private bool enableDebug = true;

    [SerializeField] private Collider2D sprCol;
    public Collider2D SprCol => sprCol;
    public Transform BottomTransform => transform;
    public Transform VisulaRootTransform;
    public Transform BackSpriteTransform;

    public TargetDetector MoveDetector;
    public GameObject SelectIndicator;
    public Transform SelectIndicatorParent;
    public IInputProvider InputProvider;
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
    public Vector2 MoveVelocity=>MoveComponent.IntentDirection * MoveComponent.MoveSpeed;
    private Transform _lastInteractSource;
    private float _initialHeightY;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponentInChildren<SpriteRenderer>();
        Ani = GetComponentInChildren<Animator>();
        _initialHeightY = Spr.transform.localPosition.y;
    }
    private void OnEnable()
    {
        if(PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);
        if(PlayerInputManager.Instance != null) PlayerInputManager.Instance.SelectPlayer(this);
        TryBindSkillSliderUI();

    }
    private void OnDisable()
    {
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Unregister(this);
    }
    private void Start()
    {
        if (GameManager.Instance != null) SelectIndicator = Instantiate(GameManager.Instance.PrefabConfig.SelectionIndicatorPrefab, SelectIndicatorParent);
        SelectIndicator.SetActive(false);
    }
    private void Update()
    {
        if (SkillComponent != null) SkillComponent.Tick();
        if (ActionLockComponent != null) ActionLockComponent.Tick();

        if (InputProvider != PlayerInputManager.Instance && AIComponent != null) AIComponent.Tick();
        SkillComponent.TickCooldownTimer();
    }
    private void LateUpdate() {
        StateComponent.DebugState();
    }


    private void FixedUpdate()
    {
        if (MoveComponent != null) MoveComponent.FixedTick();
        if (HeightComponent != null) HeightComponent.FixedTick();
        if(!StateComponent.IsMoving && !StateComponent.IsAttackingIntent) AnimationComponent.PlayIdle();
    }
    public void Initialize(PlayerStatsRuntime stats)
    {
        Rt = stats;

        //注意依賴順序
        StateComponent = new StateComponent(DebugContext.Player,Rt.StatsData.Id);
        ActionLockComponent = new ActionLockComponent(this,StateComponent);
        HealthComponent = new HealthComponent(Rt, StateComponent);
        AnimationComponent = new AnimationComponent(Ani, transform, Rb, StateComponent);

        HeightComponent = new HeightComponent(Spr.transform,StateComponent, AnimationComponent,this, Rt.StatsData);
        EffectComponent = new EffectComponent(Rt.VisualData, transform, this, Spr, StateComponent);
        RespawnComponent = new RespawnComponent(this, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb, Rt.StatsData, this, MoveDetector, AnimationComponent,HeightComponent, StateComponent);
        SpawnerComponent = new SpawnerComponent();
        if (EnemyListManager.Instance.TargetList == null) Debug.Log("EnemyListManager未初始化");
        SkillComponent = new SkillComponent(Rt.StatsData, Rt.SkillSlotCount, Rt.SkillPool, AnimationComponent, StateComponent, transform, sprCol.transform,
            EnemyListManager.Instance.TargetList,MoveComponent,HeightComponent);
        AIComponent = new AIComponent(SkillComponent, MoveComponent, transform, Rt.MoveStrategy);
        GrowthComponent = new GrowthComponent(Rt);
        //額外初始化設定
        BehaviorTree behaviourTree = PlayerBehaviourTreeFactory.Create(Rt.PlayerBehaviourTreeType, AIComponent, MoveComponent, SkillComponent, Rt.MoveStrategy);
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
        SkillComponent.OnSkillAnimationPlayed += SetFacingRight;
        MoveComponent.OnMoveDirectionChanged += SetFacingRight;

        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"玩家ID_{Rt.StatsData.Id}:({Rt.StatsData.Name})";

        TryBindSkillSliderUI();
        ResetState();
    }
    public void Interact(InteractInfo info)
    {
        _lastInteractSource = info.Source;
        if(info.KnockbackForce!=Vector2.zero) MoveComponent.Knockbacked(info.KnockbackForce, info.Source);

        MoveComponent.StopSkillDashMoveCoroutine();
        HeightComponent.StopSkillDashMoveCoroutine();
        HeightComponent.StopRecoverHeightCoroutine();

        //HeightComponent.Hurt(0.5f);
        HeightComponent.AddUpVelocity(info.FloatPower);
        //ActionLockComponent.HurtLock(0.5f);
        //AnimationComponent.PlayImmediate("Hurt");

        HealthComponent.TakeDamage(info.Damage);
        EffectComponent.TakeDamageEffect(info.Damage);

    }


    //事件方法
    public void OnDie()
    {

        AnimationComponent.PlayDie();
        StartCoroutine(Die());
    }
    private IEnumerator Die()
    {
        AIComponent.DisableAI();
        EffectComponent.PlayerDeathEffect();

        if (_lastInteractSource != null)
        {
            Vector2 dir = _lastInteractSource.position - transform.position;
            SetFacingRight(dir); // 重用你現有的方向翻轉方法
        }

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
        StateComponent.SetIsPlayingAttackAnimation( false);
        MoveComponent.ResetVelocity();
        HealthComponent.ResetCurrentHp();
    }

    public void SetInputProvider(IInputProvider inputProvider)
    {
        InputProvider = inputProvider;
    }
    private void SetFacingRight(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var scale = VisulaRootTransform.localScale;
            float mag = Mathf.Abs(scale.x);
            scale.x = (direction.x < 0f) ? -mag : mag;
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