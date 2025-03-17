using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour, IDamageable, IAttackable
{
    #region 公開變數
    public int playerID;

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
    public BehaviorTree behaviorTree;
    public PlayerSkillSpawner skillSpawner;
    public ShadowController shadowController;

    public event Action<int, int> Event_HpChanged; // (當前血量, 最大血量)
    #endregion

    #region 生命週期
    private void Awake() {
    }

    private IEnumerator Start() {
        SetBehaviorTree();
        //if (playerStats == null)
        //{
            yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        //    playerStats = PlayerStateManager.Instance.playerStatesDtny[playerID];
        //}       //設定初始化playerStats
    }

    private void Update() {
        behaviorTree.Tick(); // 執行行為樹
        UpdateCooldowns();   // 每幀更新技能冷卻時間
    }
    #endregion



    #region SetBehaviorTree()
    private void SetBehaviorTree() {
        behaviorTree.SetRoot(new Selector(new List<Node> // Selector 來處理優先級
        {
        new Action_Attack(this, 4),
        new Action_Attack(this, 3),
        new Action_Attack(this, 2),
        new Action_Attack(this, 1),
        new Action_Move(),
        new Action_Idle()
        })); ;
    }
    #endregion

    #region CanUseSkill(int skillSlot)
    public bool CanUseSkill(int skillSlot) {
        switch (skillSlot)
        {
            case 1: return CanUseSkillSlot(skillSlot1DetectPrefab, skillSlot1CurrentCooldownTime);
            case 2: return CanUseSkillSlot(skillSlot2DetectPrefab, skillSlot2CurrentCooldownTime);
            case 3: return CanUseSkillSlot(skillSlot3DetectPrefab, skillSlot3CurrentCooldownTime);
            case 4: return CanUseSkillSlot(skillSlot4DetectPrefab, skillSlot4CurrentCooldownTime);
            default: return false;
        }
    }

    private bool CanUseSkillSlot(GameObject detectPrefab, float cooldown) {
        if (detectPrefab == null) return false;
        TargetDetector detector = detectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && cooldown <= 0;
    }
    #endregion
    #region UseSkill(int skillSlot)
    public void UseSkill(int skillSlot) {
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

        // 🔹 **立即開始冷卻計時**
        currentCooldown = baseCooldown;

        Attack(slotIndex, detectPrefab, skillData.skillPrefab);
    }
    #endregion
    #region 冷卻時間倒數
    private void UpdateCooldowns() {
        // 🔹 **讓冷卻時間減少**
        skillSlot1CurrentCooldownTime = Mathf.Max(0, skillSlot1CurrentCooldownTime - Time.deltaTime);
        skillSlot2CurrentCooldownTime = Mathf.Max(0, skillSlot2CurrentCooldownTime - Time.deltaTime);
        skillSlot3CurrentCooldownTime = Mathf.Max(0, skillSlot3CurrentCooldownTime - Time.deltaTime);
        skillSlot4CurrentCooldownTime = Mathf.Max(0, skillSlot4CurrentCooldownTime - Time.deltaTime);
    }
    #endregion

    #region 公開Attack()方法
    private void Attack(int slotIndex, GameObject detectPrefab, GameObject skillPrefab) {
        animator.Play(Animator.StringToHash("Attack"));
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
    #region 公開Move()方法
    public void Move() {
        
    }
    #endregion

    #region 公開方法 TakeDamage()
    public void TakeDamage(
        int damage,
        float knockbackForce,

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
