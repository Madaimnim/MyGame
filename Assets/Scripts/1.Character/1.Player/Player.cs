using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    #region 公開變數
    public int playerID;
    public PlayerStateManager.PlayerStats playerStats; // 玩家 ID，由 Inspector 設定
    public PlayerAI playerAI;
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public BehaviorTree behaviorTree;
    public PlayerSkillSpawner skillSpawner;
    public SkillStrategyBase skillStrategy; // 存儲技能策略
    public ShadowController shadowController;
    public float stopMoveDragPower;

    public event Action<int, int> Event_HpChanged; // (當前血量, 最大血量)
    #endregion

    #region Awake()方法
    private void Awake() {
    }
    #endregion

    #region 
    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        playerStats = PlayerStateManager.Instance.playerStatesDtny[playerID];
        Debug.Log($"腳色名：{playerStats.playerName}");
    }
    #endregion



    #region 公開方法Initialize(PlayerStateManager.PlayerStats stats)
    //提供PlayerStateManager呼叫初始化使用
    public void Initialize(PlayerStateManager.PlayerStats stats) {
        playerStats = stats;
        transform.name = $"Player_{playerStats.playerID} ({playerStats.playerName})";
    }
    #endregion

    #region 公開方法 TakeDamage()
    public void TakeDamage(int takeDamage, float knockedForce) {
        if (playerStats == null) return;

        playerStats.currentHealth -= takeDamage;
        playerStats.currentHealth = Mathf.Clamp(playerStats.currentHealth, 0, playerStats.maxHealth);
        Debug.Log($"⚠️ 玩家 {playerStats.playerID} 受傷！當前血量: {playerStats.currentHealth}");

        Event_HpChanged?.Invoke(playerStats.currentHealth, playerStats.maxHealth); // 更新 UI 血量
        StartCoroutine(FlashWhite(0.1f)); // 執行閃白協程
        ShowDamageText(takeDamage); // 顯示傷害數字
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

    #region 動畫事件
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
        behaviorTree.canChangeAnim = true;
    }
    #endregion
}
