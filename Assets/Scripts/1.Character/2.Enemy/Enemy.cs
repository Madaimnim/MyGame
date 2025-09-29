using System;
using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;

public class Enemy :MonoBehaviour,IDamageable
{
    public EnemyStatsRuntime Runtime { get; private set; }
    public CharHealthComponent CharHealthComponent { get; protected set; }

    //IHealData
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }

    //變數
    #region 公開變數



    [Header("場景內預製體：手動賦予enemyID，new一個StatRuntime")]
    public int enemyID;
    public EnemyAI enemyAI;


    public BehaviorTree behaviorTree;
    public AudioClip deathSFX;
    public GameObject attackDetector01;



    public bool isEnemyDataReady { private set; get; } = false;


    [HideInInspector] public bool isPlayingActionAnimation = false;
    #endregion
    public int Id { get; protected set; }
    public bool IsDead { get; protected set; } = false;
    public bool IsKnockbacking { get; protected set; } = false;
    public bool IsPlayingAttackAnimation { get; protected set; } = false;
    public bool CanRunAI { get; protected set; } = false;
    public bool CanMove { get; protected set; } = true;

    public Rigidbody2D Rb { get; protected set; }
    public SpriteRenderer Spr { get; protected set; }
    public Animator Ani { get; protected set; }
    public Collider2D Col { get; protected set; }
    public ShadowController ShadowControl { get; protected set; }
    public IDamageable owner { get; protected set; }

    private void Awake() {
        Rb = GetComponent<Rigidbody2D>();
        Spr = GetComponent<SpriteRenderer>();
        Ani = GetComponent<Animator>();
        Col = GetComponent<Collider2D>();
        ShadowControl = GetComponentInChildren<ShadowController>();
    }

    private void OnEnable() {
        GameEventSystem.Instance.Event_BattleStart += EnableAIRun;
        GameEventSystem.Instance.Event_OnWallBroken += DisableAIRun;
    }
    private void OnDisable() {
        if (CharHealthComponent != null)
        {
            CharHealthComponent.OnDie -= OnEnemyDie;
            CharHealthComponent.OnHpChanged -= OnEnemyHpChanged;

        }


        GameEventSystem.Instance.Event_BattleStart -= EnableAIRun;
        GameEventSystem.Instance.Event_OnWallBroken -= DisableAIRun;
    }

    private void Start() {
        if (Runtime == null)
        {
            if (GameManager.Instance.IsAllDataLoaded)
            {
                SetEnemyData();
                isEnemyDataReady = true;
                MaxHp = Runtime.MaxHp;
                CurrentHp = MaxHp;
                if (StageLevelManager.Instance != null)
                    StageLevelManager.Instance.RegisterEnemy();
            }
            else Debug.LogWarning("GameManager資料載入未完成");
        }
        //發事件
        GameEventSystem.Instance.Event_HpChanged?.Invoke(CurrentHp, MaxHp, this);// 觸發事件，通知 UI 初始血量
    }

    private void Update() {
        if (!GameManager.Instance.GameStateSystem.IsControlEnabled) return;
    }

    public virtual void Initialize(EnemyStatsRuntime runtime, IDamageable damageable) {
        //HealthComponent
        CharHealthComponent = new CharHealthComponent(Runtime);
        CharHealthComponent.OnDie += OnEnemyDie;
        CharHealthComponent.OnHpChanged += OnEnemyHpChanged;

        Runtime = runtime;
        owner = damageable;
        Id = runtime.StatsData.Id;
    }


    public void OnEnemyDie() {

    }

    public void OnEnemyHpChanged(int currentHp,int maxHp) {

    }






    //被擊退
    protected virtual IEnumerator Knockback(float force, Vector2 knockbackDirection) {
        if (Rb != null)
        {
            IsKnockbacking = true;

            Rb.velocity = Vector2.zero; // 先清除當前速度，避免擊退力疊加
            Rb.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // 添加瞬間衝擊力
            yield return new WaitForSeconds(0.2f);
            Rb.velocity = Vector2.zero;

            IsKnockbacking = false;
        }
    }

    //閃白特效
    protected virtual IEnumerator FlashWhite(float duration) {
        if (Spr != null)
        {
            Spr.material = Runtime.VisualData.FlashMaterial;
            yield return new WaitForSeconds(duration);
            Spr.material = Runtime.VisualData.NormalMaterial;
        }
    }

    //重置狀態參數
    protected virtual void ResetState() {
        IsPlayingAttackAnimation = false;
        IsKnockbacking = false;
        IsDead = false;
    }

    //啟用AI
    protected virtual void EnableAIRun() {
        CanRunAI = true;
    }

    //禁用AI
    protected virtual void DisableAIRun() {
        CanRunAI = false;
    }

    public virtual void PlayAnimation(string animationName) {
        Ani?.Play(Animator.StringToHash(animationName));
    }
    protected void ResetMaterial() {
        if (Spr != null)
            Spr.material = Runtime.VisualData.NormalMaterial;
    }



    //受傷
    #region TakeDamage(DamageInfo info) 
    public void TakeDamage(DamageInfo info) {
        if (IsDead) return;

        if (IsDead || Runtime == null) return;
        Runtime.TakeDamage(info.damage);

        if (Runtime.CurrentHp <= 0 && !IsDead)
            Die();


        TextPopupManager.Instance.ShowDamagePopup(info.damage, transform); // 顯示傷害數字;

        StartCoroutine(Knockback(info.knockbackForce, info.knockbackDirection));
        StartCoroutine(FlashWhite(0.1f));//執行閃白協程，替換材質

    }
    #endregion
    //死亡
    #region Die()方法
    public void Die() {
        StageLevelManager.Instance?.EnemyDefeated();
        //GameManager.Instance.PlayerStateSystem.AddExpToAllPlayers(Runtime.Exp);
        TextPopupManager.Instance.ShowExpPopup(Runtime.Exp, transform.position);
        AudioManager.Instance.PlaySFX(deathSFX, 0.5f);
        EnemyStateManager.Instance.UnregisterEnemy(this);

        Destroy(gameObject, 0.2f);
    }
    #endregion


    public void Initialize(EnemyStatsRuntime stats) {
        Runtime = stats;
       
        CurrentHp = Runtime.CurrentHp;
        MaxHp = Runtime.MaxHp;

        transform.name = $"Enemy_{Runtime.StatsData.Id} ({Runtime.StatsData.Name})";
        Runtime.InitializeOwner(this);   // 保證血量初始化

        EnemyStateManager.Instance?.RegisterEnemy(this); // ← 統一註冊
        ResetMaterial();
    }

    //Todo
    //初始化Runtime，來自enemyStatesDtny[enemyID]
    private void SetEnemyData() {
        Runtime = EnemyStateManager.Instance.CreateRuntime(enemyID);
        if (Runtime == null) return;

        Id = Runtime.StatsData.Id;
        EnemyStateManager.Instance.RegisterEnemy(this);
        ResetMaterial();
    }

}
