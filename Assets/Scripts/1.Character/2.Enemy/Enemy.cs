using System;
using System.Collections;
using TMPro.Examples;
using System.Collections.Generic;
using UnityEngine;


public class Enemy :MonoBehaviour,IDamageable
{
    //封裝--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public EnemyStatsRuntime Rt { get; private set; }
    public HealthComponent HealthComponent { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public AnimationComponent AnimationComponent { get; private set; }
    public EffectComponent EffectComponent { get; private set; }
    public MoveComponent MoveComponent { get; private set; }
    public AIComponent AIComponent { get; private set; }
    public SkillComponent SkillComponent { get; private set; }
    public SpawnerComponent SpawnerComponent { get; private set; }


    //Unity元件--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public Rigidbody2D Rb { get; private set; }
    public SpriteRenderer Spr { get; private set; }
    public Animator Ani { get; private set; }
    public Collider2D Col { get; private set; }
    public ShadowController ShadowControl { get; private set; }

    //公開--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    public GameObject SelectIndicatorPoint;
    public IInputProvider InputProvider;

    private void Awake() {
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponent<SpriteRenderer>();
        Ani = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();
        ShadowControl = GetComponentInChildren<ShadowController>();
    }

    private void OnEnable() {
        //註冊UI:顯示血量、技能冷卻
        if (GameManager.Instance!= null)
            GameManager.Instance.EnemyStateSystem.RegisterEnemy(this);
    }

    private void Update() {
        UpdateFacing(MoveComponent.IntentDirection);
        MoveTickt();
        AttackTick();       //Todo
        SkillComponent.UpdateSkillSlotsCooldownTimer();
    }

    public void Initialize(EnemyStatsRuntime stats) {
        Rt = stats;
        var runner = new CoroutineRunnerAdapter(this);

        //順序
        AnimationComponent = new AnimationComponent(Ani, transform, Rb);
        EffectComponent = new EffectComponent(Rt.VisualData, transform, runner, Spr);
        HealthComponent = new HealthComponent(Rt);
        RespawnComponent = new RespawnComponent(runner,Rt.CanRespawn);
        MoveComponent = new MoveComponent(Rb,Rt.StatsData.MoveSpeed, runner);
        SpawnerComponent = new SpawnerComponent();
        SkillComponent = new SkillComponent(Rt.StatsData, Rt.SkillSlotCount,Rt.SkillPool, AnimationComponent, transform);
        AIComponent = new AIComponent(SkillComponent, MoveComponent, Rt.SkillSlotCount);

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



    public void AttackTick() {
        //Todo
    }
    public void MoveTickt() {
        MoveComponent.Move();
        if (!AnimationComponent.IsPlayingAttackAnimation && MoveComponent.IsMoving)
            AnimationComponent.Play("Move");
    }
    private void UpdateFacing(Vector2 direction) {
        if (direction.sqrMagnitude < 0.01f) return;     //避免靜止時頻繁執行

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            var s = transform.localScale;
            float mag = Mathf.Abs(s.x);
            s.x = (direction.x < 0f) ? -mag : mag;
            transform.localScale = s;
        }
    }
    public void TakeDamage(DamageInfo info) {
        HealthComponent.TakeDamage(info.Damage);
        EffectComponent.TakeDamageEffect(info);
        MoveComponent.Knockbacked(info.KnockbackForce, info.KnockbackDirection);
    }
    public void AnimationEvent_SpawnerSkill() {
        SkillComponent.UseSkill();
    }
    //事件方法
    public void OnHpChanged(int currentHp, int maxHp) { }     //Todo SliderChange
    public void OnDie() {
        AIComponent.DisableAI();
        AnimationComponent.Play("Die");

        if (RespawnComponent.CanRespawn)
        {
            foreach (var col in GetComponentsInChildren<Collider2D>())
                col.enabled = true;
            RespawnComponent.RespawnAfter(3f);
        } 
        else
        {
            Destroy(gameObject, 0.2f);
            if (GameManager.Instance != null) GameManager.Instance.EnemyStateSystem.UnregisterEnemy(this);
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
        AnimationComponent.Play("Idle");
        ShadowControl.ResetShadow();
    }
}
