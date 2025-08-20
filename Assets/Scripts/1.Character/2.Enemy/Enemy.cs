using System;
using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public class Enemy : MonoBehaviour, IDamageable, IAttackable
{
    #region 公開變數
    public int enemyID;// 敵人 ID，由Inspector設定
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public BehaviorTree behaviorTree;
    //public EnemySkillSpawner skillSpawner;
    public AudioClip deathSFX;
    public EnemyStateManager.EnemyStats enemyStats { get; private set; }

    public Action<int, int> Event_HpChanged;
    #endregion
    #region 私有變數
    private float lastActionTime;
    private bool isDead = false;
    private int currentHealth;
    #endregion

    #region 生命週期
    private void Awake() {
        lastActionTime = -Mathf.Infinity;
    }
    private void OnEnable() {}
    private void OnDisable() {}

    private IEnumerator Start() {
        if (enemyStats == null)
        {
            yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
            SetEnemyData();
            currentHealth = enemyStats.maxHealth;
            LevelManager.Instance.RegisterEnemy();
        }

        Event_HpChanged?.Invoke(currentHealth, enemyStats.maxHealth);// 觸發事件，通知 UI 初始血量
    }
    private void Update() { }
    #endregion

    #region 公開方法Initialize(EnemyStateManager.EnemyStats stats)
    //提供EnemyStateManager呼叫初始化使用
    public void Initialize(EnemyStateManager.EnemyStats stats) {
        enemyStats = stats;
        currentHealth = enemyStats.maxHealth;
        transform.name = $"Enemy_{enemyStats.enemyID} ({enemyStats.enemyName})";
    }
    #endregion

    public bool CanUseSkill(int skillSlot) {
        // Enemy 版本的技能冷卻與目標檢測
        return true;/* 檢查 Enemy 的技能槽條件 */;
    }

    public void UseSkill(int skillSlot) {
        // Enemy 版本的施放技能邏輯
    }

    #region 公開TakeDamage()方法
    public void TakeDamage(
        int damage,
        float knockbackForce,
        Vector2 knockbackDirection,

        float dotDuration,
        float dotDamage,

        float attackReduction,
        float attackReductionDuration,

        float speedReduction,
        float speedReductionDuration) {


        if (isDead) return; //  已經死了就不要再處理

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, enemyStats.maxHealth);
        Event_HpChanged?.Invoke(currentHealth, enemyStats.maxHealth);//觸發事件，通知 UI 更新血量

        StartCoroutine(Knockback(knockbackForce, knockbackDirection));
        StartCoroutine(FlashWhite(0.1f));//執行閃白協程，替換材質
        ShowDamageText(damage);//顯示damage數字TEXT

        if(currentHealth<=0)
        {
            Die();
        }
    }
    #endregion

    #region Die()方法
    private void Die (){
        isDead = true;
        LevelManager.Instance.EnemyDefeated();
        PopupManager.Instance.ShowExpPopup(enemyStats.exp, transform.position);
        AudioManager.Instance.PlaySFX(deathSFX,0.5f);
        Destroy(gameObject, 0.5f);
    }
    #endregion

    #region 私有ShowDamageText(int damage)
    private void ShowDamageText(int damage) {
        //Todo if (Stats.damageTextPrefab == null)
        //{
        //    Debug.LogError("傷害預製體未設置，請在EnemyStatData裡設置");
        //    return;
        //}
        //Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0); // 讓數字浮動在敵人上方
        //
        //GameObject damageTextObj = Instantiate(Stats.damageTextPrefab, spawnPosition, Quaternion.identity, transform);
        //damageTextObj.GetComponent<DamageTextController>().Setup(damage);
    }
    #endregion
    #region 私有SetEnemyData()
    private void SetEnemyData() {
        enemyStats = EnemyStateManager.Instance.enemyStatesDtny[enemyID];
        spriteRenderer.material = GameManager.Instance.normalMaterial;
    }
    #endregion

    #region 私有協程：FlashWhite(float duration)，受擊閃白效果
    private IEnumerator FlashWhite(float duration) {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = GameManager.Instance.flashMaterial;
            yield return new WaitForSeconds(duration);
            spriteRenderer.material = GameManager.Instance.normalMaterial;
        }
    }
    #endregion

    #region 私有協程： ApplyKnockback(float force,knockbackDirection)
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
}
