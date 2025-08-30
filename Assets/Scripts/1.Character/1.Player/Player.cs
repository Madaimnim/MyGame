using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour, IDamageable, IAttackable
{
    #region 公開變數
    public int playerID;
    public GameObject selectIndicatorPoint;

    //技能槽、冷卻倒數、預製體用變數
    #region
    // 基礎冷卻時間（不變動，從 SkillData 來)
    public float skillSlot1CooldownTime;
    public float skillSlot2CooldownTime;
    public float skillSlot3CooldownTime;
    public float skillSlot4CooldownTime;

    // 實際倒數的冷卻時間
    private float skillSlot1CurrentCooldownTime = 0;
    private float skillSlot2CurrentCooldownTime = 0;
    private float skillSlot3CurrentCooldownTime = 0;
    private float skillSlot4CurrentCooldownTime = 0;

    public GameObject skillSlot1DetectPrefab;
    public GameObject skillSlot2DetectPrefab;
    public GameObject skillSlot3DetectPrefab;
    public GameObject skillSlot4DetectPrefab;
    #endregion

    [NonSerialized] public PlayerStateManager.PlayerStatsRuntime playerStats; 
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public PlayerSkillSpawner skillSpawner;
    public ShadowController shadowController;
    public AudioClip deathSFX;
    public static event Action<IDamageable> Event_OnPlayerDie;


    public bool isKnockback = false;

    private bool onAttackRecovery = false;
    private bool isPlayingActionAnimation = false;

    private bool isDead = false;
    private int currentHealth;

    public event Action<int, int> Event_HpChanged; // (當前血量, 最大血量)
    #endregion

    //生命週期
    #region 生命週期
    private void Awake() {

    }
    private void Start() {
        //if (playerStats != null)
        //{
        //    currentHealth = playerStats.maxHealth;
        //    Event_HpChanged?.Invoke(currentHealth, playerStats.maxHealth);// 觸發事件，通知 UI 初始血量
        //}        
    }
    private void Update() {
        UpdateCooldowns();   // 每幀更新技能冷卻時間
    }

    private void OnEnable() {
        if (playerStats == null) return;

        currentHealth = playerStats.maxHealth;
        Event_HpChanged?.Invoke(currentHealth, playerStats.maxHealth);
        isDead = false;
        isKnockback = false;
        spriteRenderer.material = GameManager.Instance.normalMaterial;
        Debug.Log($"{gameObject.name} OnEnable啟用，當前血量為{currentHealth}。");
    }

    private void OnDisable() {
        onAttackRecovery = false;
        isPlayingActionAnimation = false;
        isKnockback = false;
    }
    #endregion

    //判斷技能是否冷卻完成
    #region CanUseSkill(int skillSlot)
    public bool CanUseSkill(int skillSlot) {
        bool canUse = false;

        switch (skillSlot)
        {
            case 1: canUse = CanUseSkillSlot(skillSlot1DetectPrefab, skillSlot1CurrentCooldownTime); break;
            case 2: canUse = CanUseSkillSlot(skillSlot2DetectPrefab, skillSlot2CurrentCooldownTime); break;
            case 3: canUse = CanUseSkillSlot(skillSlot3DetectPrefab, skillSlot3CurrentCooldownTime); break;
            case 4: canUse = CanUseSkillSlot(skillSlot4DetectPrefab, skillSlot4CurrentCooldownTime); break;
        }

   
        return canUse;
    }

    private bool CanUseSkillSlot(GameObject detectPrefab, float cooldown) {
        if (detectPrefab == null) return false;
        TargetDetector detector = detectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && cooldown <= 0;
    }
    #endregion

    //技能施放
    #region  UseSkill(int skillSlot)
    public void UseSkill(int skillSlot) {
        if (onAttackRecovery)
        {
            Debug.Log($"Debug: UseSkill({skillSlot}) 被阻止，因為攻擊硬直中");
            return;
        }

        switch (skillSlot)
        {
            case 1: UseSkillSlot(0, skillSlot1DetectPrefab, ref skillSlot1CurrentCooldownTime, skillSlot1CooldownTime); break;
            case 2: UseSkillSlot(1, skillSlot2DetectPrefab, ref skillSlot2CurrentCooldownTime, skillSlot2CooldownTime); break;
            case 3: UseSkillSlot(2, skillSlot3DetectPrefab, ref skillSlot3CurrentCooldownTime, skillSlot3CooldownTime); break;
            case 4: UseSkillSlot(3, skillSlot4DetectPrefab, ref skillSlot4CurrentCooldownTime, skillSlot4CooldownTime); break;
        }
    }

    private void UseSkillSlot(int slotIndex, GameObject detectPrefab, ref float currentCooldown, float baseCooldown) {
        if (playerStats == null) return;
        var skillData = playerStats.GetSkillAtSkillSlot(slotIndex);
        if (skillData == null) return;

        currentCooldown = baseCooldown;
        Attack(slotIndex, detectPrefab, skillData.skillPrefab);
    }
    #endregion

    //冷卻倒數
    #region 冷卻時間倒數
    private void UpdateCooldowns() {
        skillSlot1CurrentCooldownTime = Mathf.Max(0, skillSlot1CurrentCooldownTime - Time.deltaTime);
        skillSlot2CurrentCooldownTime = Mathf.Max(0, skillSlot2CurrentCooldownTime - Time.deltaTime);
        skillSlot3CurrentCooldownTime = Mathf.Max(0, skillSlot3CurrentCooldownTime - Time.deltaTime);
        skillSlot4CurrentCooldownTime = Mathf.Max(0, skillSlot4CurrentCooldownTime - Time.deltaTime);

        
    }
    #endregion

    //攻擊
    #region 公開Attack()方法
    private void Attack(int slotIndex, GameObject detectPrefab, GameObject skillPrefab) {
        if (onAttackRecovery || isPlayingActionAnimation)
        {
            Debug.Log($"Attack() 被阻止，因為 onAttackRecovery={onAttackRecovery}, isPlayingActionAnimation={isPlayingActionAnimation}");
            return;
        }

        SkillObject skillObject = skillPrefab.GetComponent<SkillObject>();        
        if (skillObject == null) return;
        TargetDetector targetDetector= detectPrefab.GetComponent<TargetDetector>();
        if (targetDetector == null) return;
        if (targetDetector.targetTransform == null)
        {
            Debug.LogWarning("Attack()中止：targetTransform 已被 Destroy 或不存在");
            return;
        }

        // 翻轉角色朝向
        bool isTargetOnLeft = targetDetector.targetTransform.position.x < transform.position.x;
        transform.localScale = new Vector3(isTargetOnLeft ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                                     transform.localScale.y,
                                     transform.localScale.z);
        isPlayingActionAnimation = true;
        onAttackRecovery = true;
        string attackAnimationName = GetAttackAnimationName(skillObject.attackAnimationType);

        animator.Play(Animator.StringToHash(attackAnimationName));
        StartCoroutine(WaitAndStartAnimationLock( skillObject.attackRecoveryTime));
        StartCoroutine(PlayAnimationAndSpawnSkillAfterDelay(skillObject.attackSpawnDelayTime, skillPrefab, detectPrefab));
    }
    #endregion

    //取得攻擊動畫名稱
    #region GetAttackAnimationName(SkillObject.AttackAnimationType type)
    private string GetAttackAnimationName(SkillObject.AttackAnimationType type) {
        switch (type)
        {
            case SkillObject.AttackAnimationType.Attack01: return "Attack01";
            case SkillObject.AttackAnimationType.Attack02: return "Attack02";
            case SkillObject.AttackAnimationType.Other01: return "Other01";
            case SkillObject.AttackAnimationType.Other02: return "Other02";
            default: return "Attack01";
        }
    }
    #endregion
    //等待攻擊動畫結束後重置可撥放動畫，等待攻擊冷卻結束後重置可攻擊
    #region IEnumerator WaitAndStartAnimationLock(float attackRecoveryTime)
    private IEnumerator WaitAndStartAnimationLock(float attackRecoveryTime) {
        yield return null; // 等待一幀，確保動畫已切換
        float animationDurationTime = GetCurrentAnimationLength();

        yield return new WaitForSeconds(animationDurationTime);
        isPlayingActionAnimation = false;

        yield return new WaitForSeconds(attackRecoveryTime);
        onAttackRecovery = false;
    }
    #endregion
    //撥放技能動畫，並生成技能
    #region PlayAnimationAndSpawnSkillAfterDelay(float delayTime, GameObject skillPrefab, GameObject detectPrefab)
    private IEnumerator PlayAnimationAndSpawnSkillAfterDelay(float delayTime, GameObject skillPrefab, GameObject detectPrefab) {
        yield return new WaitForSeconds(delayTime);
        if (skillSpawner != null && detectPrefab != null)
        {
            TargetDetector detector = detectPrefab.GetComponent<TargetDetector>();
            if (detector != null && detector.targetTransform != null)
            {
                skillSpawner.SpawnAttack(skillPrefab, detector.targetTransform);
            }
        }
    }
    #endregion
    //取得攻擊動畫時長
    #region GetCurrentAnimationLength()
    private float GetCurrentAnimationLength() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 獲取當前動畫狀態
        return stateInfo.length; // 直接返回動畫時長
    }
    #endregion

    //受傷
    #region 公開方法 TakeDamage()
    public void TakeDamage(int damage,float knockbackForce,Vector2 knockbackDirection) {
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (playerStats == null) return;
        isKnockback = true;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, playerStats.maxHealth);      
        Event_HpChanged?.Invoke(currentHealth, playerStats.maxHealth); // 更新 UI 血量
        StartCoroutine(FlashWhite(0.1f)); // 執行閃白協程
        StartCoroutine(Knockback(knockbackForce, knockbackDirection));
        ShowDamageText(damage); // 顯示傷害數字
        Debug.Log($"{gameObject.name}受到攻擊，受到{damage}點傷害，血量:{currentHealth}");


    }
    #endregion

    //死亡
    #region Die()方法
    private void Die() {
        isDead = true;
        
        VFXManager.Instance.Play("PlayerDeath",transform.position);
        AudioManager.Instance.PlaySFX(deathSFX, 0.5f);
        Event_OnPlayerDie?.Invoke(this);

        gameObject.SetActive(false);
    }
    #endregion

    //被擊退特效
    #region IEnumerator Knockback(float force,Vector2 knockbackDirection)
    private IEnumerator Knockback(float force, Vector2 knockbackDirection) {
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 先清除當前速度，避免擊退力疊加

            rb.AddForce(force * knockbackDirection, ForceMode2D.Impulse); // 添加瞬間衝擊力
            yield return new WaitForSeconds(0.2f);
            rb.velocity = Vector2.zero;
            isKnockback = false;
        }
    }
    #endregion
    //受傷特效
    #region 閃白受擊
    private IEnumerator FlashWhite(float duration) {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = GameManager.Instance.flashMaterial;
            yield return new WaitForSeconds(duration);
            spriteRenderer.material = GameManager.Instance.normalMaterial;
        }
    }
    #endregion
    //顯示傷害數字
    #region 顯示傷害數字
    private void ShowDamageText(int damage) {

        Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0); // 讓數字浮動在玩家上方

    }
    #endregion

    public void AdjustShadowAlpha() {
        if (shadowController != null)
            shadowController.AdjustShadowAlpha();
    }

    public void StartMoving() {
        Debug.Log("✅ 動畫事件 StartMoving");
    }

    public void StopMoving() {
        Debug.Log("✅ 動畫事件 StopMoving");
    }

    public void ResetCanChangeAnim() {
        Debug.Log("✅ 動畫事件 ResetCanChangeAnim");
        //behaviorTree.canChangeAnim = true;
    }

    //提供PlayerStateManager呼叫初始化使用
    #region 公開方法Initialize(PlayerStateManager.PlayerStatsRuntime stats)
    public void Initialize(PlayerStateManager.PlayerStatsRuntime stats) {
        playerStats = stats;
        transform.name = $"Player_{playerStats.playerID} ({playerStats.playerName})";
    }
    #endregion
}
