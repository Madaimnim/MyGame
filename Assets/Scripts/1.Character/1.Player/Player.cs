using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour, IDamageable, IAttackable
{
    #region 公開變數
    public int playerID;
    public GameObject selectIndicatorPoint;

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

    public PlayerStateManager.PlayerStats playerStats; // 玩家 ID，由 Inspector 設定
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public PlayerSkillSpawner skillSpawner;
    public ShadowController shadowController;


    private bool onAttackRecovery = false;
    private bool isPlayingActionAnimation = false;

    public event Action<int, int> Event_HpChanged; // (當前血量, 最大血量)
    #endregion

    #region 生命週期
    private void Awake() {
    }

    private void Start() {
    }

    private void Update() {
        UpdateCooldowns();   // 每幀更新技能冷卻時間
    }
    #endregion

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

    #region 技能施放
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

    #region 冷卻時間倒數
    private void UpdateCooldowns() {
        skillSlot1CurrentCooldownTime = Mathf.Max(0, skillSlot1CurrentCooldownTime - Time.deltaTime);
        skillSlot2CurrentCooldownTime = Mathf.Max(0, skillSlot2CurrentCooldownTime - Time.deltaTime);
        skillSlot3CurrentCooldownTime = Mathf.Max(0, skillSlot3CurrentCooldownTime - Time.deltaTime);
        skillSlot4CurrentCooldownTime = Mathf.Max(0, skillSlot4CurrentCooldownTime - Time.deltaTime);

        
    }
    #endregion

    #region 公開Attack()方法
    private void Attack(int slotIndex, GameObject detectPrefab, GameObject skillPrefab) {
        if (onAttackRecovery || isPlayingActionAnimation)
        {
            Debug.Log($"🚫 Debug: Attack() 被阻止，因為 onAttackRecovery={onAttackRecovery}, isPlayingActionAnimation={isPlayingActionAnimation}");
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
    private IEnumerator WaitAndStartAnimationLock(float attackRecoveryTime) {
        yield return null; // 等待一幀，確保動畫已切換
        float animationDurationTime = GetCurrentAnimationLength();


        yield return new WaitForSeconds(animationDurationTime);
        isPlayingActionAnimation = false;

        yield return new WaitForSeconds(attackRecoveryTime);
        onAttackRecovery = false;
    }

    #region 延遲生成技能
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

    private float GetCurrentAnimationLength() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 🔹 獲取當前動畫狀態
        return stateInfo.length; // 直接返回動畫時長
    }

    #endregion

    //受傷
    #region 公開方法 TakeDamage()
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
        if (playerStats == null) return;

        playerStats.currentHealth -= damage;
        playerStats.currentHealth = Mathf.Clamp(playerStats.currentHealth, 0, playerStats.maxHealth);      
        Event_HpChanged?.Invoke(playerStats.currentHealth, playerStats.maxHealth); // 更新 UI 血量
        StartCoroutine(FlashWhite(0.1f)); // 執行閃白協程
        ShowDamageText(damage); // 顯示傷害數字
    }
    #endregion
    //顯示傷害數字
    #region 顯示傷害數字
    private void ShowDamageText(int damage) {
        //Todo if (playerState == null || playerState.damageTextPrefab == null)
        //{
        //    Debug.LogError("❌ [Player] 傷害數字預製體未設置！");
        //    return;
        //}

        Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0); // 讓數字浮動在玩家上方
        //Todo GameObject damageTextObj = Instantiate(playerState.damageTextPrefab, spawnPosition, Quaternion.identity, transform);
        //damageTextObj.GetComponent<DamageTextController>().Setup(damage);
    }
    #endregion
    //受傷特效
    #region 閃白受擊
    private IEnumerator FlashWhite(float duration) {
        if (spriteRenderer != null )
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

    #region 公開方法Initialize(PlayerStateManager.PlayerStats stats)
    //提供PlayerStateManager呼叫初始化使用
    public void Initialize(PlayerStateManager.PlayerStats stats) {
        playerStats = stats;
        transform.name = $"Player_{playerStats.playerID} ({playerStats.playerName})";
    }
    #endregion
}
