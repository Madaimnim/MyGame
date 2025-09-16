using System;
using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour, IDamageable
{
    //變數
    #region 公開變數
    public int enemyID;// 敵人 ID，由Inspector設定
    public EnemyAI enemyAI;
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public BehaviorTree behaviorTree;
    public AudioClip deathSFX;
    public GameObject attackDetector01;
    public ShadowController shadowController;
    public EnemyStateManager.EnemyStatsRuntime enemyStats { get; private set; }

    private bool isDead = false;
    private int currentHealth;
    public bool canRunAI { get; private set; } = false;


    public bool isEnemyDataReady { private set; get; } = false;
    

    [HideInInspector] public bool isPlayingActionAnimation = false;
    #endregion

    //生命週期
    #region 生命週期
    private void Awake() {}
    private void OnEnable() {
        EventManager.Instance.Event_BattleStart += EnableAIRun;
        EventManager.Instance.Event_OnWallBroken += DisableAIRun;
    }
    private void OnDisable() {
        EventManager.Instance.Event_BattleStart -= EnableAIRun;
        EventManager.Instance.Event_OnWallBroken -= DisableAIRun;
    }

    private IEnumerator Start() {
        if (enemyStats == null)
        {
            yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
            SetEnemyData();
            isEnemyDataReady = true;
            currentHealth = enemyStats.maxHealth;
            if (StageLevelManager.Instance!=null)
                StageLevelManager.Instance.RegisterEnemy();
        }

        EventManager.Instance.Event_HpChanged?.Invoke(currentHealth, enemyStats.maxHealth,this);// 觸發事件，通知 UI 初始血量
    }
    private void Update() { }
    #endregion

    //撥放攻擊動畫
    #region PlayAnmitaion(string animationName)
    public void PlayAnimation(string animationName) {
        animator.Play(Animator.StringToHash(animationName));
    }

    #endregion

    //受傷
    #region TakeDamage(int damage,float knockbackForce,Vector2 knockbackDirection,float dotDuration,float dotDamage,float attackReduction,float attackReductionDuration,float speedReduction,        float speedReductionDuration) 
    public void TakeDamage(DamageInfo info) {
        if (isDead) return; //  已經死了就不要再處理

        currentHealth -= info.damage;
        TextPopupManager.Instance.ShowDamagePopup(info.damage, transform); // 顯示傷害數字;

        currentHealth = Mathf.Clamp(currentHealth, 0, enemyStats.maxHealth);
        EventManager.Instance.Event_HpChanged?.Invoke(currentHealth, enemyStats.maxHealth, this);// 觸發事件，通知 UI 初始血量

        StartCoroutine(Knockback(info.knockbackForce, info.knockbackDirection));
        StartCoroutine(FlashWhite(0.1f));//執行閃白協程，替換材質

        if(currentHealth<=0)
        {
            Die();
        }
    }
    #endregion
    //死亡
    #region Die()方法
    private void Die (){
        isDead = true;
        StageLevelManager.Instance?.EnemyDefeated();
        ExpManager.Instance?.ExpToAllPlayer(enemyStats.exp);

        TextPopupManager.Instance.ShowExpPopup(enemyStats.exp, transform.position);
        AudioManager.Instance.PlaySFX(deathSFX,0.5f);
        Destroy(gameObject, 0.2f);
    }
    #endregion

    //初始化enemyStats，由EnemyStateManager執行方法初始化
    #region Initialize(EnemyStateManager.EnemyStats stats)
    public void Initialize(EnemyStateManager.EnemyStatsRuntime stats) {
        enemyStats = stats;
        currentHealth = enemyStats.maxHealth;
        transform.name = $"Enemy_{enemyStats.enemyID} ({enemyStats.enemyName})";
    }
    #endregion

    //初始化enemyStats，來自enemyStatesDtny[enemyID]
    #region SetEnemyData()
    private void SetEnemyData() {
        enemyStats = EnemyStateManager.Instance.enemyStatesDtny[enemyID];
        spriteRenderer.material = GameManager.Instance.normalMaterial;
    }
    #endregion

    //閃白特效
    #region IEnumerator FlashWhite(float duration)
    private IEnumerator FlashWhite(float duration) {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = GameManager.Instance.flashMaterial;
            yield return new WaitForSeconds(duration);
            spriteRenderer.material = GameManager.Instance.normalMaterial;
        }
    }
    #endregion
    //被擊退特效
    #region IEnumerator Knockback(float force,Vector2 knockbackDirection)
    private IEnumerator Knockback(float force,Vector2 knockbackDirection) {
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // ✅ 先清除當前速度，避免擊退力疊加
            rb.AddForce( force* knockbackDirection, ForceMode2D.Impulse); // ✅ 添加瞬間衝擊力
            yield return new WaitForSeconds(0.2f);
            rb.velocity = Vector2.zero;
        }
    }
    #endregion

    //啟用AI
    #region EnableAIRun(IDamageable damagealbe)
    private void EnableAIRun() {
        canRunAI = true;
    }
    #endregion
    //禁用AI
    #region DisableAIRun(IDamageable damagealbe)
    private void DisableAIRun() {
        canRunAI = false;
    }
    #endregion

    //Todo設定影子變化
    #region 公有AdjustShadowAlpha()方法，AnimationEvent調用
    public void AdjustShadowAlpha() {
        if (shadowController != null)
        {
            shadowController.AdjustShadowAlpha();
        }
        else
            Debug.LogError("shadowController為空");
    }
    #endregion
}
