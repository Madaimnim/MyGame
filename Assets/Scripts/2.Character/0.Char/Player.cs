using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Player : MonoBehaviour, IInteractable, IAnimationEventOwner
{
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [SerializeField] private Collider2D sprCol;
    public Collider2D SprCol => sprCol;
    public Transform BottomTransform => RootSpriteTransform;
    public Transform RootSpriteTransform;
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
        //註冊UI:顯示血量、技能冷卻
        if (UIManager_BattlePlayer.Instance != null)UIManager_BattlePlayer.Instance.RegisterPlayerPanelUI(this);
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Register(this);
    }
    private void OnDisable()
    {
        if (UIManager_BattlePlayer.Instance != null)UIManager_BattlePlayer.Instance.UnregisterPlayer(this);
        if (PlayerListManager.Instance != null) PlayerListManager.Instance.Unregister(this);
    }
    private void Start()
    {
        if (GameManager.Instance != null) SelectIndicator = Instantiate(GameManager.Instance.PrefabConfig.SelectionIndicatorPrefab, SelectIndicatorParent);
        SelectIndicator.SetActive(false);
    }
    private void Update()
    {
        if (MoveComponent != null && !StateComponent.IsDead)SetFacingRight(MoveComponent.IntentDirection); 
        if (SkillComponent != null) SkillComponent.Tick();
        if (ActionLockComponent != null) ActionLockComponent.Tick();

        if (InputProvider != PlayerInputManager.Instance && AIComponent != null) AIComponent.Tick();
        SkillComponent.TickCooldownTimer();
    }
    private void FixedUpdate()
    {
        if (MoveComponent != null) MoveComponent.Tick();
    }
    public void Initialize(PlayerStatsRuntime stats)
    {
        Rt = stats;

        //注意依賴順序
        StateComponent = new StateComponent();
        ActionLockComponent = new ActionLockComponent(this,StateComponent);
        HealthComponent = new HealthComponent(Rt, StateComponent);
        AnimationComponent = new AnimationComponent(Ani, transform, Rb, StateComponent);
        HeightComponent = new HeightComponent(Spr.transform, this, _initialHeightY,Rt.StatsData.MoveSpeed,StateComponent);
        EffectComponent = new EffectComponent(Rt.VisualData, transform, this, Spr, StateComponent);
        RespawnComponent = new RespawnComponent(this, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb, Rt.StatsData.MoveSpeed, this, MoveDetector, AnimationComponent,HeightComponent, StateComponent);
        SpawnerComponent = new SpawnerComponent();
        if (EnemyListManager.Instance.TargetList == null) Debug.Log("EnemyListManager未初始化");
        SkillComponent = new SkillComponent(Rt.StatsData, Rt.SkillSlotCount, Rt.SkillPool, AnimationComponent, StateComponent, BackSpriteTransform, EnemyListManager.Instance.TargetList);
        AIComponent = new AIComponent(SkillComponent, MoveComponent, transform, Rt.MoveStrategy);
        GrowthComponent = new GrowthComponent(Rt);
        //額外初始化設定
        BehaviorTree behaviourTree = PlayerBehaviourTreeFactory.Create(Rt.PlayerBehaviourTreeType, AIComponent, MoveComponent, SkillComponent, Rt.MoveStrategy);
        AIComponent.SetBehaviorTree(behaviourTree);

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


        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"玩家ID_{Rt.StatsData.Id}:({Rt.StatsData.Name})";

        ResetState();
    }
    public void Interact(InteractInfo info)
    {
        _lastInteractSource = info.Source;
        HealthComponent.TakeDamage(info.Damage);
        EffectComponent.TakeDamageEffect(info.Damage);
        MoveComponent.Knockbacked(info.KnockbackForce, info.Source);
        HeightComponent.FloatUp(info.FloatPower);

        StateComponent.SetIsPlayingAttackAnimation(false);
    }
    public void AnimationEvent_SpawnerSkill()
    {
        SkillComponent.UseSkill();
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
        if (StateComponent.IsPlayingAttackAnimation) return;  //確保攻擊時面相正確
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var s = transform.localScale;
            float mag = Mathf.Abs(s.x);
            s.x = (direction.x < 0f) ? -mag : mag;
            transform.localScale = s;
        }
    }
}