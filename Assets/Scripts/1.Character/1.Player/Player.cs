using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour, IDamageable, IAttackable
{
    #region 公開變數
    public int playerID;
    public float skillSlot1CooldownTime;
    public float skillSlot2CooldownTime;
    public float skillSlot3CooldownTime;
    public float skillSlot4CooldownTime;

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

    #region CanUseSkillSlot(int skillSlot)          bool 
    public bool CanUseSkill(int skillSlot) {
        switch (skillSlot)
        {
            case 1: return CanUseSkillSlot1();
            case 2: return CanUseSkillSlot2();
            case 3: return CanUseSkillSlot3();
            case 4: return CanUseSkillSlot4();
            default: return false;
        }
    }

    public bool CanUseSkillSlot1() {
        if (skillSlot1DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }
    public bool CanUseSkillSlot2() {
        if (skillSlot2DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }
    public bool CanUseSkillSlot3() {
        if (skillSlot3DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }
    public bool CanUseSkillSlot4() {
        if (skillSlot4DetectPrefab == null) return false;
        TargetDetector detector = skillSlot1DetectPrefab.GetComponent<TargetDetector>();
        return detector != null && detector.hasTarget && skillSlot1CooldownTime <= 0;
    }
    #endregion
    #region UseSkillSlot(int skillSlot)             void
    public void UseSkill(int skillSlot) {
        switch (skillSlot)
        {
            case 1: UseSkillSlot1(); break;
            case 2: UseSkillSlot2(); break;
            case 3: UseSkillSlot3(); break;
            case 4: UseSkillSlot4(); break;
        }
    }

    public void UseSkillSlot1() { skillSlot1CooldownTime = playerStats.GetSkillAtSkillSlot(0).cooldownTime; Attack(); }
    public void UseSkillSlot2() { skillSlot2CooldownTime = playerStats.GetSkillAtSkillSlot(1).cooldownTime; Attack(); }
    public void UseSkillSlot3() { skillSlot3CooldownTime = playerStats.GetSkillAtSkillSlot(2).cooldownTime; Attack(); }
    public void UseSkillSlot4() { skillSlot4CooldownTime = playerStats.GetSkillAtSkillSlot(3).cooldownTime; Attack(); }
    #endregion

    #region 公開Attack()方法
    public void Attack() {
        animator.Play(Animator.StringToHash("Attack"));
        //behaviorTree.canChangeAnim = false;
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
