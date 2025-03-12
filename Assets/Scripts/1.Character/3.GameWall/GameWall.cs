using System;
using System.Collections;
using UnityEngine;

public class GameWall : MonoBehaviour, IDamageable
{
    #region 公開變數
    public int gameWallID=1;            
    public string gameWallName ="CommonWall";
    public int maxHp =10;
    public int attackPower =1;
    public float coolDownTime =0.1f;
    public float flashWhiteTime =0.1f;
    public GameObject damageTextPrefab; // 存儲讀取的傷害數字預製體

    public event Action<int, int> ValueChanged;//int參數代表(當前血量, 最大血量)`
    #endregion

    #region 私有變數
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int takeDamage;
    private int currentHp;
    #endregion

    #region Awake()方法
    private void Awake() {
    }
    #endregion

    #region Start()方法
    private void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHp = maxHp;
        ValueChanged?.Invoke(currentHp, maxHp);// 觸發事件，通知 UI 初始血量
    }
    #endregion
        
    #region TakeDamage()方法，

    public void TakeDamage(int damage,float knockedForece) {
        takeDamage = damage;
        currentHp -= takeDamage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        Debug.Log($"敵人 {gameWallID} 受傷！當前血量: {currentHp}");

        ValueChanged?.Invoke(currentHp, maxHp);//觸發事件，通知 UI 更新血量

        ShowDamageText(takeDamage);//顯示damage數字TEXT
    }
    #endregion

    #region 私有ShowDamageText(int damage)
    private void ShowDamageText(int damage) {
        if (damageTextPrefab == null)
        {
            Debug.LogError("傷害預製體未設置，請在GameWallStatData裡設置");
            return;
        }
        Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0); // ✅ 讓數字浮動在敵人上方

        //Instantiate 傷害TEXT預製體
        GameObject damageTextObj = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, transform);
        damageTextObj.GetComponent<DamageTextController>().Setup(damage);
    }
    #endregion

}
