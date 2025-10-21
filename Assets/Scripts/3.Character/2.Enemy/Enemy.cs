using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy :MonoBehaviour,IInteractable
{
    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Collider2D BottomCollider => _bottomCollider;
    [SerializeField] private Collider2D _bottomCollider;

    public TargetDetector MoveDetector;
    public IInputProvider InputProvider;
    //組件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Rigidbody2D Rb;
    public Animator Ani;
    public SpriteRenderer Spr;
    public Collider2D Col;
    public ShadowController ShadowControl;
    //模組化 --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public EnemyStatsRuntime Rt { get; private set; }
    public HealthComponent HealthComponent { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public AnimationComponent AnimationComponent { get; private set; }
    public EffectComponent EffectComponent { get; private set; }
    public AIComponent AIComponent { get; private set; }
    public MoveComponent MoveComponent { get; private set; }
    public SkillComponent SkillComponent { get; private set; }
    public SpawnerComponent SpawnerComponent { get; private set; }
    public bool IsDead => HealthComponent.IsDead;
    private Transform _lastInteractSource;

    public void Initialize(EnemyStatsRuntime stats) {
        Rt = stats;

        //順序
        HealthComponent = new HealthComponent(Rt);
        AnimationComponent = new AnimationComponent(Ani, transform, Rb);
        EffectComponent = new EffectComponent(Rt.VisualData, transform, this, Spr, HealthComponent);

        RespawnComponent = new RespawnComponent(this, Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb,Rt.StatsData.MoveSpeed, this, MoveDetector, AnimationComponent,BottomCollider);
        SpawnerComponent = new SpawnerComponent();
        SkillComponent = new SkillComponent(Rt.StatsData, Rt.SkillSlotCount,Rt.SkillPool, AnimationComponent, transform);
        AIComponent = new AIComponent(SkillComponent, MoveComponent, transform,Rt.MoveStrategyType);
        //額外初始化設定
        AnimationComponent.Initial(MoveComponent);

        //事件訂閱
        HealthComponent.OnDie += OnDie;
        HealthComponent.OnHpChanged += OnHpChanged;
        RespawnComponent.OnRespawn += OnRespawn;
        if (GameEventSystem.Instance != null)
        {
            GameEventSystem.Instance.Event_BattleStart += AIComponent.EnableAI;
            GameEventSystem.Instance.Event_OnWallBroken += AIComponent.DisableAI;
            GameEventSystem.Instance.Event_BattleStart += RespawnComponent.EnableRespawn;
            GameEventSystem.Instance.Event_OnWallBroken += RespawnComponent.DisableRespawn;
        }

        //初始化狀態--------------------------------------------------------------------------------------------------------------------------------------------------------------------
        transform.name = $"EnemyID_{Rt.StatsData.Id}:({Rt.StatsData.Name})";
        InputProvider = AIComponent;
        ResetState();
    }
    public void AnimationTick() {
        if (AnimationComponent.IsPlayingAttackAnimation) return;
        if (SkillComponent.IntentSlotIndex >= 0) return;
        if (HealthComponent.IsDead) return;

        if (MoveComponent.IsMoving ) AnimationComponent.PlayMove();
    }

    public void Interact(InteractInfo info)
    {
        _lastInteractSource= info.Source;
        HealthComponent.TakeDamage(info.Damage);
        EffectComponent.TakeDamageEffect(info.Damage);
        MoveComponent.Knockbacked(info.KnockbackForce, info.Source);
    }
    public void AnimationEvent_SpawnerSkill() {
        SkillComponent.UseSkill();
    }
    //事件方法
    public void OnHpChanged(int currentHp, int maxHp) { }     //Todo SliderChange
    public void OnDie() {
        AnimationComponent.PlayDie();
        StartCoroutine(Die());
    }
    private IEnumerator Die()
    {
        AIComponent.DisableAI();
        MoveComponent.DisableMove();

        if (_lastInteractSource != null)
        {
            Vector2 dir = _lastInteractSource.position - transform.position;
            Debug.Log($"敵人死亡面相更新來源:{dir}");
            SetFacingLeft(dir); // 重用你現有的方向翻轉方法
        }

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        yield return null; //等一幀，讓Animator確實切到Die動畫
        AnimatorStateInfo state = Ani.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length / Ani.speed);

        if (RespawnComponent.CanRespawn) RespawnComponent.RespawnAfter(3f);
        else
        {
            Destroy(gameObject);
            if (GameManager.Instance != null)
                GameManager.Instance.EnemyStateSystem.UnregisterEnemy(this);
        }
    }

    public void OnRespawn() {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = true;
        ResetState();
        AIComponent.EnableAI();
    }
    public void ResetState() {
        AnimationComponent.IsPlayingAttackAnimation = false;
        MoveComponent.Reset();
        HealthComponent.ResetCurrentHp();
        ShadowControl.ResetShadow();
    }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponent<SpriteRenderer>();
        Ani = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();
        ShadowControl = GetComponentInChildren<ShadowController>();
    }
    private void OnEnable()
    {
        //註冊UI:顯示血量、技能冷卻
        if (GameManager.Instance != null)
            GameManager.Instance.EnemyStateSystem.RegisterEnemy(this);
    }
    private void Start()
    {
        InputProvider = AIComponent;
    }
    private void Update()
    {
        if (MoveComponent != null && !IsDead) SetFacingLeft(MoveComponent.IntentDirection);
        if (SkillComponent != null) SkillComponent.Tick();
        if (AnimationComponent != null) AnimationTick();
        if (AIComponent != null) AIComponent.Tick();
    }
    private void FixedUpdate()
    {
        MoveComponent.Tick();
    }

    private void SetFacingLeft(Vector2 direction)
    {
        if (AnimationComponent.IsPlayingAttackAnimation) return;  //確保攻擊時面相正確
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var s = transform.localScale;
            float mag = Mathf.Abs(s.x);
            s.x = (direction.x < 0f) ? mag : -mag;
            transform.localScale = s;

            if (IsDead) Debug.Log($"更新敵人死亡面相:{transform.localScale}，");
        }
    }
}
