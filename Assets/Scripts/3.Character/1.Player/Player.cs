using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class Player : MonoBehaviour, IInteractable
{
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Collider2D BottomCollider => _bottomCollider;
    [SerializeField] private Collider2D _bottomCollider;

    public TargetDetector MoveDetector;
    public GameObject SelectIndicator;
    public Transform SelectIndicatorParent;
    public IInputProvider InputProvider;
    //組件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Rigidbody2D Rb;
    public Animator Ani;
    public SpriteRenderer Spr;
    public Collider2D Col;
    public ShadowController ShadowControl;
    //模組化 --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public PlayerStatsRuntime Rt { get; private set; }
    public HealthComponent HealthComponent { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public AnimationComponent AnimationComponent { get; private set; }
    public EffectComponent EffectComponent { get; private set; }
    public AIComponent AIComponent { get; private set; }
    public MoveComponent MoveComponent { get; private set; }
    public SkillComponent SkillComponent { get; private set; }
    public SpawnerComponent SpawnerComponent { get; private set; }
    public GrowthComponent GrowthComponent { get; private set; }

    public void Initialize(PlayerStatsRuntime stats)
    {
        Rt = stats;
        var runner = new CoroutineRunnerAdapter(this);

        //順序
        AnimationComponent = new AnimationComponent(Ani, transform, Rb);
        EffectComponent = new EffectComponent(Rt.VisualData, transform, runner, Spr);
        HealthComponent = new HealthComponent(Rt);
        RespawnComponent = new RespawnComponent(runner, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb, Rt.StatsData.MoveSpeed, runner, MoveDetector, AnimationComponent, BottomCollider);
        SpawnerComponent = new SpawnerComponent();
        SkillComponent = new SkillComponent(Rt.StatsData, Rt.SkillSlotCount, Rt.SkillPool, AnimationComponent, transform);
        AIComponent = new AIComponent(SkillComponent, MoveComponent, transform, Rt.MoveStrategyType);
        GrowthComponent = new GrowthComponent(Rt);
        //額外初始化設定
        AnimationComponent.Initial(MoveComponent);

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
        HealthComponent.TakeDamage(info.Damage);
        EffectComponent.TakeDamageEffect(info.Damage);
        MoveComponent.Knockbacked(info.KnockbackForce, info.Source);
    }
    public void AnimationEvent_SpawnerSkill()
    {
        SkillComponent.UseSkill();
    }
    public void AnimationTick()
    {
        if (!AnimationComponent.IsPlayingAttackAnimation &&
            MoveComponent.IsMoving &&
            SkillComponent.IntentSlotIndex < 0)

            AnimationComponent.PlayMove();
    }


    //事件方法
    public void OnDie()
    {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        MoveComponent.DisableMove();
        AIComponent.DisableAI();

        AnimationComponent.PlayDie();
        EffectComponent.PlayerDeathEffect();

        RespawnComponent.RespawnAfter(3f);

        //發事件
        GameEventSystem.Instance.Event_OnPlayerDie?.Invoke(this);
    }
    public void OnHpChanged(int currentHp, int maxHp) { }    //Todo SliderChange
    public void OnRespawn()
    {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = true;

        ResetState();

        AIComponent.EnableAI();
    }
    public void ResetState()
    {
        AnimationComponent.IsPlayingAttackAnimation = false;
        MoveComponent.Reset();
        HealthComponent.ResetCurrentHp();
        ShadowControl.ResetShadow();
    }

    public void SetInputProvider(IInputProvider inputProvider)
    {
        InputProvider = inputProvider;
    }
    private void UpdateFacing(Vector2 direction)
    {
        if (AnimationComponent.IsPlayingAttackAnimation) return;  //確保攻擊時面相正確
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var s = transform.localScale;
            float mag = Mathf.Abs(s.x);
            s.x = (direction.x < 0f) ? -mag : mag;
            transform.localScale = s;
        }
    }

    private void Awake(){
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponent<SpriteRenderer>();
        Ani = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();
        ShadowControl = GetComponentInChildren<ShadowController>();
    }
    private void OnEnable(){
        //註冊UI:顯示血量、技能冷卻
        if (UIManager_BattlePlayer.Instance != null)
            UIManager_BattlePlayer.Instance.RegisterPlayerPanelUI(this);
    }
    private void OnDisable(){
        if (UIManager_BattlePlayer.Instance != null)
            UIManager_BattlePlayer.Instance.UnregisterPlayer(this);
    }
    private void Start(){
        if (GameManager.Instance != null) SelectIndicator = Instantiate(GameManager.Instance.PrefabConfig.SelectionIndicatorPrefab, SelectIndicatorParent);
        SelectIndicator.SetActive(false);
    }
    private void Update(){
        if (MoveComponent != null) UpdateFacing(MoveComponent.IntentDirection);
        if (SkillComponent != null) SkillComponent.Tick();
        if (AnimationComponent != null) AnimationTick();
        Debug.Log($"InputProvider:{InputProvider== PlayerInputManager.Instance},AIComponent{AIComponent!=null}");
        if (InputProvider != PlayerInputManager.Instance && AIComponent != null) AIComponent.Tick();
    }
    private void FixedUpdate(){
        if (MoveComponent != null) MoveComponent.Tick();
    }
}