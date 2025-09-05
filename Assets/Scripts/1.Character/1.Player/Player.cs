using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour, IDamageable, IAttackable
{
    //SkillSLot類
    #region  class SkillSlot
    [Serializable]
    public class SkillSlot
    {
        [HideInInspector] public float cooldown;         // 固定冷卻時間
        [HideInInspector] public float cooldownTimer; // 倒數
        [HideInInspector] public GameObject detectPrefab;    // 偵測用物件
    }
    #endregion

    //變數
    #region
    public int playerID;
    public GameObject selectIndicatorPoint;

    public SkillSlot[] skillSlots = new SkillSlot[4]; // 四個技能槽統一管理

    public PlayerStateManager.PlayerStatsRuntime playerStats{ get; private set; }
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public PlayerSkillSpawner skillSpawner;
    public ShadowController shadowController;
    public AudioClip deathSFX;
    public static event Action<IDamageable> Event_OnPlayerDie;


    public bool isKnockback = false;

    public bool isPlayingAttackAnimation { get; private set; } = false;

    private bool isDead = false;
    private int currentHealth;

    public event Action<int, int> Event_HpChanged; // (當前血量, 最大血量)
    #endregion

    //生命週期
    #region 生命週期
    private void Awake() {

    }

    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        
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
        isPlayingAttackAnimation = false;
        isKnockback = false;
    }
    #endregion



    //判斷技能是否冷卻完成
    #region CanUseSkill(int skillSlot)
    public bool CanUseSkill(int skillSlot) {
        int slotIndex = skillSlot - 1;
        if (IsSlotIndexWrong(slotIndex)) return false;
        
        var slot = skillSlots[slotIndex];
        if (slot.detectPrefab == null) return false;

        var detector = slot.detectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && slot.cooldownTimer <= 0;
    }

    //BehaviorTree的Action_Attack執行判斷
    public void UseSkill(int slotSlot) {
        int slotIndex = slotSlot-1;
        if (IsSlotIndexWrong(slotIndex)) return;
        if (isPlayingAttackAnimation)
        {
            Debug.Log($"UseSkill({slotIndex}) 被阻止：攻擊動畫中");
            return;
        }

        var slot = skillSlots[slotIndex];
        var skillData = playerStats?.GetSkillAtSkillSlot(slotIndex);
        if (skillData == null) return;

        // 設置冷卻
        slot.cooldownTimer = slot.cooldown;

        // 播放攻擊
        Attack(slotIndex, slot.detectPrefab, skillData.skillPrefab);
    }

    private bool IsSlotIndexWrong(int slotIndex) {
        return slotIndex < 0 && slotIndex >= skillSlots.Length;
    }


    // 技能冷卻更新 
    private void UpdateCooldowns() {
        foreach (var slot in skillSlots)
        {
            slot.cooldownTimer = Mathf.Max(0, slot.cooldownTimer - Time.deltaTime);
        }
    }
    #endregion


    public void SetPlayingAttackAnimation(bool b) {
        isPlayingAttackAnimation = b;
    }

    //攻擊
    #region 公開Attack()方法
    private void Attack(int slotIndex, GameObject detectPrefab, GameObject skillPrefab) {
        if (DialogueManager.Instance.isDialogueRunning) return;
        if (isPlayingAttackAnimation)
        {
            Debug.Log($"Attack() 被阻止，isPlayingActionAnimation={ isPlayingAttackAnimation}");
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
        string attackAnimationName = GetAttackAnimationName(skillObject.attackAnimationType);

        animator.Play(Animator.StringToHash(attackAnimationName));
        StartCoroutine(PlayAnimationAndSpawnSkillAfterDelay(slotIndex,skillObject.attackSpawnDelayTime, skillPrefab, detectPrefab));
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

    //撥放技能動畫，並生成技能
    #region PlayAnimationAndSpawnSkillAfterDelay(float delayTime, GameObject skillPrefab, GameObject detectPrefab)
    private IEnumerator PlayAnimationAndSpawnSkillAfterDelay(int slotIndex,float delayTime, GameObject skillPrefab, GameObject detectPrefab) {
        yield return new WaitForSeconds(delayTime);
        if (skillSpawner != null && detectPrefab != null)
        {
            TargetDetector detector = detectPrefab.GetComponent<TargetDetector>();
            if (detector != null && detector.targetTransform != null)
            {
                skillSpawner.SpawnAttack(playerStats.attackPower,skillPrefab, detector.targetTransform,playerStats.GetSkillAtSkillSlot(slotIndex).attack);

                int currentSkillUsageCount=playerStats.GetSkillAtSkillSlot(slotIndex).skillUsageCount++;
                int nextSkillUsageCount = playerStats.GetSkillAtSkillSlot(slotIndex).nextSkillLevelCount;
                if (currentSkillUsageCount >= nextSkillUsageCount)
                    SkillLevelUp(slotIndex);
            }
        }
    }
    //技能升級
    #region SkillLevelUp(){}
    public void SkillLevelUp(int slotIndex) {
        playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel++;
        playerStats.GetSkillAtSkillSlot(slotIndex).attack++;
        playerStats.GetSkillAtSkillSlot(slotIndex).nextSkillLevelCount += playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel * 10;
        TextPopupManager.Instance.ShowSkillLevelUpPopup(playerStats.GetSkillAtSkillSlot(slotIndex).skillName, playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel, transform);
    }
    #endregion

    #endregion



    //受傷
    #region 公開方法 TakeDamage()
    public void TakeDamage(DamageInfo info) {
        if (playerStats == null) return;
        isKnockback = true;

        currentHealth -= info.damage;
        TextPopupManager.Instance.ShowTakeDamagePopup(info.damage, transform); // 顯示傷害數字

        currentHealth = Mathf.Clamp(currentHealth, 0, playerStats.maxHealth);      
        Event_HpChanged?.Invoke(currentHealth, playerStats.maxHealth); // 更新 UI 血量

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(FlashWhite(0.1f)); // 執行閃白協程
        StartCoroutine(Knockback(info.knockbackForce, info.knockbackDirection));

        Debug.Log($"{gameObject.name}受到攻擊，受到{info.damage}點傷害，血量:{currentHealth}");

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

    public void AdjustShadowAlpha() {
        if (shadowController != null)
            shadowController.AdjustShadowAlpha();
    }

    //提供PlayerStateManager呼叫初始化使用
    #region 公開方法Initialize(PlayerStateManager.PlayerStatsRuntime stats)
    public void Initialize(PlayerStateManager.PlayerStatsRuntime stats) {
        playerStats = stats;
        transform.name = $"Player_{playerStats.playerID} ({playerStats.playerName})";
    }
    #endregion
}
