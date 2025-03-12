using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    #region 私有變數
    private Vector2 moveDirection;
    private float moveSpeed;
    private float knockForce;
    private float destroySelfDelay; // 自身摧毀時間
    private int attackPower = 0; // 攻擊力
    private LayerMask targetLayers; // 支援多選 Layer

    private Rigidbody2D rb;
    private float destroyDelay = 0.1f; // 碰撞後摧毀延遲
    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>(); // 確保每個敵人只觸發一次
    #endregion

    #region 生命週期
    private void Awake() {
        rb = GetComponent<Rigidbody2D>(); // 獲取剛體
    }

     private void Start() {
        Destroy(gameObject, destroySelfDelay);
    }

    private void Update() {
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
    }
    #endregion

    #region 設定技能屬性
    public void SetSkillProperties(Vector2 direction, float speed, float force, float destroyDelay, int power, LayerMask layers) {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        knockForce = force;
        destroySelfDelay = destroyDelay;
        attackPower = power;
        targetLayers = layers;
    }
    #endregion

    #region OnTriggerEnter2D

    private void OnTriggerEnter2D(Collider2D collision) {
        // ✅ 使用 LayerMask 來檢查是否在攻擊目標內
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && !hitTargets.Contains(damageable))
            {
                hitTargets.Add(damageable);
                damageable.TakeDamage(attackPower, knockForce);
                Destroy(gameObject, destroyDelay);
            }
        }
    }
    #endregion
}
