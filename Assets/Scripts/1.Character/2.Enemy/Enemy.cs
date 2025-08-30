using System;
using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour, IDamageable, IAttackable
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
    public EnemyStateManager.EnemyStatsRuntime enemyStats { get; private set; }
    public Action<int, int> Event_HpChanged;

    private bool isDead = false;
    private int currentHealth;

    private IDamageable currentTargetClass;
    private Transform currentTargetTransform;
    private int currentAttackPower;
    private float currentKnockbackForce;
    private Vector3 currentKnockbackDirection;
    private GameObject currentAttackPrefab;

    [HideInInspector] public bool isPlayingActionAnimation = false;
    #endregion

    //生命週期
    #region 生命週期
    private void Awake() {    }
    private void OnEnable() {}
    private void OnDisable() {}

    private IEnumerator Start() {
        if (enemyStats == null)
        {
            yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
            SetEnemyData();
            currentHealth = enemyStats.maxHealth;
            StageLevelManager.Instance.RegisterEnemy();
        }

        Event_HpChanged?.Invoke(currentHealth, enemyStats.maxHealth);// 觸發事件，通知 UI 初始血量
    }
    private void Update() { }
    #endregion

    //繼承IAttackable需實現方法
    #region CanUseSkill(int skillSlot)、UseSkill(int skillSlot)
    public bool CanUseSkill(int skillSlot) {
        //if()
        return true;
    }

    public void UseSkill(int skillSlot) {
        // Enemy 版本的施放技能邏輯
    }
    #endregion

    //請求攻擊
    #region RequestAttack(int slotID, Transform targetTransform,string animationName,IDamageable player)
    public void RequestAttack(int slotID, Transform targetTransform,string animationName,IDamageable playerClass) {
        if (isPlayingActionAnimation) return;
        
        currentTargetClass = playerClass;
        currentTargetTransform = targetTransform;
        currentAttackPower = enemyStats.skillPoolDtny[slotID].attackPower;
        currentKnockbackForce = enemyStats.skillPoolDtny[slotID].knockbackForce;
        currentKnockbackDirection = new Vector3(targetTransform.position.x - transform.position.x, targetTransform.position.y - transform.position.y, 0).normalized;
        currentAttackPrefab = enemyStats.skillPoolDtny[slotID].attackPrefab;
        bool isTargetOnRight = targetTransform.position.x > transform.position.x;
        transform.localScale = new Vector3(isTargetOnRight ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                                     transform.localScale.y,
                                     transform.localScale.z);
        animator.Play(Animator.StringToHash(animationName));     
    }
    #endregion

    //攻擊
    #region Attack()
    public void Attack() {
        if (currentTargetClass == null) return;
        var targetMono = currentTargetClass as MonoBehaviour;
        if (targetMono == null || !targetMono.gameObject.activeInHierarchy)
        {
            // ❌ 目標已經死掉或被關閉，直接放棄
            currentTargetClass = null;
            return;
        }

        currentTargetClass.TakeDamage(currentAttackPower, currentKnockbackForce, currentKnockbackDirection);
    }
    #endregion

    //生成技能
    #region SpawnSkill()
    private void SpawnSkill(GameObject attackPrefab) {
            if (attackPrefab != null)
            {
                //todo
            }
    }
    #endregion

    //(保留備用，暫時用不到)取得攻擊動畫時長
    #region GetCurrentAnimationLength()
    private float GetCurrentAnimationLength() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        int hash = stateInfo.shortNameHash;
        // 轉回字串 (需要你自己對照 AnimatorController 裡的名字)
        string clipName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        Debug.Log($"當前動畫為{clipName}，其時長為{stateInfo.length}");
        return stateInfo.length; // 直接返回動畫時長
    }
    #endregion
    //受傷
    #region TakeDamage(int damage,float knockbackForce,Vector2 knockbackDirection,float dotDuration,float dotDamage,float attackReduction,float attackReductionDuration,float speedReduction,        float speedReductionDuration) 
    public void TakeDamage(int damage,float knockbackForce,Vector2 knockbackDirection) {
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
    //死亡
    #region Die()方法
    private void Die (){
        isDead = true;
        StageLevelManager.Instance.EnemyDefeated();

        ExpManager.Instance?.ExpToAllPlayer(enemyStats.exp);

        TextPopupManager.Instance.ShowExpPopup(enemyStats.exp, transform.position);
        AudioManager.Instance.PlaySFX(deathSFX,0.5f);
        Destroy(gameObject, 0.5f);
    }
    #endregion

    //Todo
    #region ShowDamageText(int damage)
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

    //Todo 簡化
    //初始化enemyStats，由外部執行方法初始化
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
}
