using UnityEngine;
using System.Collections.Generic;
using System.Threading;

[DisallowMultipleComponent]

public class EnemySkillSlot : MonoBehaviour
{
    #region 公開變數
    
    public int slotID;
    public string animationName;
    public Enemy enemy;
    public Transform targetTransform;

    private readonly HashSet<IDamageable> targetPlayersHashSet = new();

    [Header("偵測目標")] 
    public LayerMask targetLayers;


    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;

    #endregion

    //生命週期
    #region //生命週期
    private void OnEnable() {
        Player.Event_OnPlayerDie += HandlePlayerDie;
    }
    private void OnDisable() {
        Player.Event_OnPlayerDie -= HandlePlayerDie;
    }

    private void Update() {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f) isOnCooldown = false;
            return;
        }


        foreach (var playerClass in targetPlayersHashSet)
        {
            var targetTransform = ((MonoBehaviour)playerClass).transform;
            float cooldownTime = enemy.enemyStats.skillPoolDtny[slotID].cooldownTime;

            enemy.RequestAttack(slotID, targetTransform, animationName, playerClass);

            isOnCooldown = true;
            cooldownTimer = cooldownTime;
            break; // 一次只打一個
        }
    }
    #endregion
    //Triiger觸發器
    #region OnTriggerStay2D(Collider2D other)
    private void OnTriggerEnter2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & targetLayers.value) == 0) return;
        var playerClass = col.GetComponent<IDamageable>();
        if (playerClass != null) targetPlayersHashSet.Add(playerClass);
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (((1 << col.gameObject.layer) & targetLayers.value) == 0) return;
        var playerClass = col.GetComponent<IDamageable>();
        if (playerClass != null) targetPlayersHashSet.Remove(playerClass);
    }
    #endregion

    //清除偵測目標玩家HashSet，避免錯誤
    #region HandlePlayerDie(IDamageable player)
    private void HandlePlayerDie(IDamageable player) {
        targetPlayersHashSet.Remove(player);
    }
    #endregion

    #region 設定技能屬性
    private void SetSkillObjectProperties(GameObject currentSkillPrefab, Vector2 moveDirection, int baseAttack, Transform targetTransform, float rotateAngle) {

    }
    #endregion
}
