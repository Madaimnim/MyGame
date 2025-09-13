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
        [HideInInspector] public int skillID;          // 插槽裝的技能
        [HideInInspector] public float cooldown;       // 固定冷卻時間
        [HideInInspector] public float cooldownTimer;  // 倒數
        [HideInInspector] public GameObject detectPrefab; // 偵測用物件     
    }
    #endregion

    //變數
    #region
    public int playerID;
    public GameObject selectIndicatorPoint;

    public SkillSlot[] skillSlots = new SkillSlot[4];       // 四個技能槽統一管理

    public PlayerStateManager.PlayerStatsRuntime playerStats{ get; private set; }
    public Animator animator;
    public Collider2D collider;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public PlayerSkillSpawner skillSpawner;
    public PlayerMove playerMove;

    public ShadowController shadowController;
    public AudioClip deathSFX;



    public bool canRunAI { get; private set; } = false;
    public bool canRespawn { get; private set; } = true;
    public bool isKnockback { get; private set; } = false;
    public bool isDead { get; private set; } = false;
    public bool isPlayingAttackAnimation { get; private set; } = false;
    public bool isMoving { get; private set; } = false;
    public string GetSkillName(int slotIndex) => playerStats.GetSkillNameAtSlot(slotIndex);
    public int GetSkillLevel(int slotIndex) => playerStats.GetSkillLevelAtSlot(slotIndex);

    private int currentHealth;

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

        if (UIManager_Player.Instance != null)
        {
            StartCoroutine(DelayedRegisterUI());
        }


        currentHealth = playerStats.maxHealth;
        spriteRenderer.material = GameManager.Instance.normalMaterial;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.Event_HpChanged?.Invoke(currentHealth, playerStats.maxHealth, this);
            EventManager.Instance.Event_BattleStart += EnableAIRun;
            EventManager.Instance.Event_BattleStart += EnableRespawn;
            EventManager.Instance.Event_OnWallBroken += DisableAIRun;
            EventManager.Instance.Event_OnWallBroken += DisableRespawn;
        }
            

    }

    private void OnDisable() {
        if (UIManager_Player.Instance != null)
        {
            UIManager_Player.Instance.UnregisterPlayer(this);
        }

        isPlayingAttackAnimation = false;
        isKnockback = false;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.Event_BattleStart -= EnableAIRun;
            EventManager.Instance.Event_BattleStart -= EnableRespawn;
            EventManager.Instance.Event_OnWallBroken -= DisableAIRun;
            EventManager.Instance.Event_OnWallBroken -= DisableRespawn;
        }
    }
    #endregion
    //延遲註冊UI
    #region IEnumerator DelayedRegisterUI()
    private IEnumerator DelayedRegisterUI() {
        yield return null; // 等一幀，確保 Instantiate 流程跑完
        UIManager_Player.Instance.RegisterPlayer(this);
    }
    #endregion

    //BehaviorTree判斷技能是否冷卻完成
    #region CanUseSkill(int skillSlot)
    public bool CanUseSkill(int skillSlot) {
        int slotIndex = skillSlot - 1;
        if (IsSlotIndexWrong(slotIndex)) return false;
        
        var slot = skillSlots[slotIndex];
        if (slot.detectPrefab == null) return false;

        var detector = slot.detectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && slot.cooldownTimer <= 0;
    }
    #endregion

    //BehaviorTree的Action_Attack執行判斷
    #region  UseSkill(int slotSlot)
    public void UseSkill(int slotSlot) {
        int slotIndex = slotSlot-1;
        if (IsSlotIndexWrong(slotIndex)) return;
        if (isPlayingAttackAnimation)
        {
            
            //return;
        }

        var slot = skillSlots[slotIndex];
        var skillData = playerStats?.GetSkillAtSkillSlot(slotIndex);
        if (skillData == null) return;

        // 設置冷卻
        slot.cooldownTimer = slot.cooldown;

        // 播放攻擊
        Attack(slotIndex, slot.detectPrefab, skillData.skillPrefab);
    }
    #endregion

    //確認SlotIndex是否正確
    #region IsSlotIndexWrong(int slotIndex)
    private bool IsSlotIndexWrong(int slotIndex) {
        return slotIndex < 0 && slotIndex >= skillSlots.Length;
    }
    #endregion

    // 技能冷卻更新 
    #region UpdateCooldowns()
    private void UpdateCooldowns() {
        for(int i = 0; i < skillSlots.Length; i++)
        {
            var slot = skillSlots[i];
            if (slot == null) continue;
            slot.cooldownTimer = Mathf.Max(0,slot.cooldownTimer-Time.deltaTime);
            EventManager.Instance.Event_SkillCooldownChanged?.Invoke(i, slot.cooldownTimer, slot.cooldown, this);
        }
    }
    #endregion

    //設置是否在攻擊動畫
    #region SetPlayingAttackAnimation(bool b)
    public void SetPlayingAttackAnimation(bool b) {
        isPlayingAttackAnimation = b;
    }
    #endregion

    //攻擊
    #region 公開Attack()方法
    private void Attack(int slotIndex, GameObject detectPrefab, GameObject skillPrefab) {
        if (DialogueManager.Instance.isDialogueRunning) return;
        if (isPlayingAttackAnimation)
        {
           
            //return;
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


        string animName = skillObject.GetAnimationName(isMoving);
        animator.Play(Animator.StringToHash(animName),-1,0f);

        StartCoroutine(skillSpawner.SpawnSkillAfterDelay(slotIndex,skillObject.attackSpawnDelayTime, skillPrefab, detectPrefab));
    }
    #endregion

    //技能升級
    #region SkillLevelUp(){}
    public void SkillLevelUp(int slotIndex) {
        playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel++;
        playerStats.GetSkillAtSkillSlot(slotIndex).attack++;
        playerStats.GetSkillAtSkillSlot(slotIndex).nextSkillLevelCount += playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel * 10;
        TextPopupManager.Instance.ShowSkillLevelUpPopup(playerStats.GetSkillAtSkillSlot(slotIndex).skillName, playerStats.GetSkillAtSkillSlot(slotIndex).currentLevel, transform);
        EventManager.Instance.Event_SkillInfoChanged?.Invoke(slotIndex, this);
    }
    #endregion

    //收到控制指令嘗試移動
    #region TryMove(Vector2 direction)
    public void TryMove(Vector2 direction) {
        // 如果死了 / 被擊退 ，就不允許移動
        if (isDead || isKnockback) return;

        isMoving = direction != Vector2.zero;

        if (!isPlayingAttackAnimation && isMoving)
        {
            animator.Play(Animator.StringToHash("Move"));
        }
        playerMove.Move(direction,this,rb);
    }
    #endregion

    //受傷
    #region 公開方法 TakeDamage()
    public void TakeDamage(DamageInfo info) {
        if (playerStats == null) return;
        if (isDead) return;
        isKnockback = true;

        currentHealth -= info.damage;
        TextPopupManager.Instance.ShowTakeDamagePopup(info.damage, transform); // 顯示傷害數字

        currentHealth = Mathf.Clamp(currentHealth, 0, playerStats.maxHealth);
        EventManager.Instance.Event_HpChanged?.Invoke(currentHealth, playerStats.maxHealth,this); // 更新 UI 血量

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
        //Debug.Log("玩家死亡");
        isDead = true;
        
        VFXManager.Instance.Play("PlayerDeath",transform.position);
        AudioManager.Instance.PlaySFX(deathSFX, 0.5f);

        DisableAIRun();

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0.5f;  // 透明度 0=完全透明，1=完全不透明，你可以調整成 0.3~0.7
            spriteRenderer.color = c;
        }

        animator.Play(Animator.StringToHash("Die"));
        shadowController.SetShadowOffset();

        EventManager.Instance.Event_OnPlayerDie?.Invoke(this);
    }
    #endregion
    //復活
    #region Respawn()
    public void Respawn() {
        Debug.Log("玩家復活");
        isDead = false;
        isKnockback = false;
        currentHealth = playerStats.maxHealth;
        EventManager.Instance.Event_HpChanged?.Invoke(currentHealth, playerStats.maxHealth,this);

        //Todo:VFXManager.Instance.Play("PlayerDeath", transform.position);
        //Todo:AudioManager.Instance.PlaySFX(deathSFX, 0.5f);
        EnableAIRun();

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        //：恢復角色完全不透明
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        animator.Play(Animator.StringToHash("Idle"));
        shadowController.ResetShadow();
    }
    #endregion

    //被擊退
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
    //受傷閃白協程
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

    //提供PlayerStateManager呼叫初始化使用
    #region Initialize(PlayerStateManager.PlayerStatsRuntime stats)
    public void Initialize(PlayerStateManager.PlayerStatsRuntime stats) {
        playerStats = stats;
        transform.name = $"Player_{playerStats.playerID} ({playerStats.playerName})";
    }
    #endregion

    //啟用AI
    #region EnableAIRun
    private void EnableAIRun() {
        canRunAI = true;
    }
    #endregion
    //禁用AI
    #region DisableAIRun()
    private void DisableAIRun() {
        canRunAI = false;
    }
    #endregion


    //啟用復活
    #region DisableAIRun()
    private void EnableRespawn() {
        canRespawn = true;
    }
    #endregion
    //禁用復活
    #region DisableAIRun()
    private void DisableRespawn() {
        canRespawn = false;
    }
    #endregion
}
