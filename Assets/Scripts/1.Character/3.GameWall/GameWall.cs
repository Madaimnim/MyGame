using System;
using System.Collections;
using UnityEngine;

public class GameWall : MonoBehaviour, IDamageable
{
    #region 公開變數
    public int gameWallID=1;     
    public string gameWallName ="CommonWall";
    public int maxHealth = 10;
    public int attackPower =0;
    public float coolDownTime =0f;
    public float flashWhiteTime =0.1f;
    public Material NormalMaterial;
    public Material FlashMaterial;

    #endregion

    #region 私有變數
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private bool isDefeat=false;
    #endregion

    #region Awake()方法
    private void Awake() {
    }
    #endregion

    #region Start()方法
    private void Start() {
        isDefeat=false;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        GameEventSystem.Instance.Event_HpChanged?.Invoke(currentHealth, maxHealth,this);// 觸發事件，通知 UI 初始血量
    }
    #endregion

    //受傷
    #region 公開方法 TakeDamage()
    public void TakeDamage(DamageInfo info) {
        if (isDefeat) return;

        currentHealth -= info.damage;
        TextPopupManager.Instance.ShowTakeDamagePopup(info.damage, transform); // 顯示傷害數字

        currentHealth = Mathf.Clamp(currentHealth, 0,maxHealth);
        GameEventSystem.Instance.Event_HpChanged?.Invoke(currentHealth, maxHealth,this); // 更新 UI 血量

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(FlashWhite(0.1f)); // 執行閃白協程


    }
    #endregion

    public void Die() {
        isDefeat = true;
        GameEventSystem.Instance.Event_OnWallBroken.Invoke();
    }

    //受傷特效
    #region 閃白受擊
    private IEnumerator FlashWhite(float duration) {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = FlashMaterial;
            yield return new WaitForSeconds(duration);
            spriteRenderer.material = NormalMaterial;
        }
    }
    #endregion
}