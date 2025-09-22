using System;
using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;

public class Enemy : Char<EnemyStatsRuntime,EnemySkillRuntime>
{
    //變數
    #region 公開變數
    [Header("場景內預製體：手動賦予enemyID，new一個StatRuntime")]
    public int enemyID;
    public EnemyAI enemyAI;
    public int CurrentHp { get; private set; }
    public int MaxHp { get; private set; }

    public BehaviorTree behaviorTree;
    public AudioClip deathSFX;
    public GameObject attackDetector01;



    public bool isEnemyDataReady { private set; get; } = false;


    [HideInInspector] public bool isPlayingActionAnimation = false;
    #endregion

    //生命週期
    #region 生命週期
    protected override void Awake() {
        base.Awake();

    }

    protected override void OnEnable() {
        base.OnEnable();

        EventManager.Instance.Event_BattleStart += EnableAIRun;
        EventManager.Instance.Event_OnWallBroken += DisableAIRun;
    }
    private void OnDisable() {
        EventManager.Instance.Event_BattleStart -= EnableAIRun;
        EventManager.Instance.Event_OnWallBroken -= DisableAIRun;
    }

    private IEnumerator Start() {
        if (StatsRuntime == null)
        {
            yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
            SetEnemyData();
            isEnemyDataReady = true;
            MaxHp = StatsRuntime.MaxHp;
            CurrentHp = MaxHp;
            if (StageLevelManager.Instance != null)
                StageLevelManager.Instance.RegisterEnemy();
        }

        EventManager.Instance.Event_HpChanged?.Invoke(CurrentHp, MaxHp, this);// 觸發事件，通知 UI 初始血量
    }
    private void Update() { }
    #endregion

    //受傷
    #region TakeDamage(DamageInfo info) 
    public override void TakeDamage(DamageInfo info) {
        if (IsDead) return; 

        base.TakeDamage(info);


        TextPopupManager.Instance.ShowDamagePopup(info.damage, transform); // 顯示傷害數字;

        StartCoroutine(Knockback(info.knockbackForce, info.knockbackDirection));
        StartCoroutine(FlashWhite(0.1f));//執行閃白協程，替換材質

    }
    #endregion
    //死亡
    #region Die()方法
    protected override void Die() {
        base.Die();
        StageLevelManager.Instance?.EnemyDefeated();
        PlayerStateManager.Instance.AddExpToAllPlayers(StatsRuntime.Exp);
        TextPopupManager.Instance.ShowExpPopup(StatsRuntime.Exp, transform.position);
        AudioManager.Instance.PlaySFX(deathSFX, 0.5f);
        EnemyStateManager.Instance.UnregisterEnemy(this);

        Destroy(gameObject, 0.2f);
    }
    #endregion


    public void Initialize(EnemyStatsRuntime stats) {
        StatsRuntime = stats;
        StatsRuntime.InitializeOwner(this);   // 保證血量初始化
        CurrentHp = StatsRuntime.CurrentHp;
        MaxHp = StatsRuntime.MaxHp;

        transform.name = $"Enemy_{StatsRuntime.Id} ({StatsRuntime.Name})";

        EnemyStateManager.Instance?.RegisterEnemy(this); // ← 統一註冊
    }

    //Todo
    //初始化StatsRuntime，來自enemyStatesDtny[enemyID]
    private void SetEnemyData() {
        StatsRuntime = EnemyStateManager.Instance.CreateRuntime(enemyID);
        if (StatsRuntime == null) return;

        Id = StatsRuntime.Id;
        EnemyStateManager.Instance.RegisterEnemy(this);
    }

}
