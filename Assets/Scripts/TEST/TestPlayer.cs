using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{   [Header("基本資訊")]
    public string entityName = "Entity";
    public LayerMask targetLayers;
    public List<TestPlayer> TestPlayerList;


    [Header("底部Collider（請從Inspector拖入）")]
    public Collider2D bottomCollider;

    [Header("攻擊設定")]
    public float horizontalForce = 1f;   // 🔹 X 軸擊退力
    public float verticalForce = 1f;     // 🔹 初始向上力

    [Header("重力設定")]
    public float gravityScale = 15f;     //重力強度，可在 Inspector 調整

    [Header("受擊反應設定")]
    private float groundY = float.NaN;
    private Coroutine knockbackCoroutine;
    private bool isKnocked = false;


    private void Update() {
        TestPlayerList.RemoveAll(t => t == null);

        List<TestPlayer> waitToRemove = new List<TestPlayer>();

        foreach (var testPlayer in TestPlayerList)
        {
            bool isTouch = bottomCollider.IsTouching(testPlayer.bottomCollider);
            if (isTouch)
            {
                waitToRemove.Add(testPlayer);

                // 判斷是否是敵人
                if (testPlayer.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    Debug.Log("有目標");
                    testPlayer.ApplyKnockback(transform.position, horizontalForce, verticalForce);
                }
            }
        }

        foreach (var t in waitToRemove)
            TestPlayerList.Remove(t);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
            return;

        var testPlayer = other.GetComponent<TestPlayer>();
        if (testPlayer == null) return;

        TestPlayerList.Add(testPlayer);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
            return;

        var testPlayer = other.GetComponent<TestPlayer>();
        if (testPlayer == null) return;

        TestPlayerList.Remove(testPlayer);

        Debug.Log($"目標離開{testPlayer.name}");
    }

    public void ApplyKnockback(Vector3 attackerPos, float xForce, float yForce) {
        if (knockbackCoroutine != null)
        {
            Debug.Log("暫停了協程");
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }
        Debug.Log("啟動協程");
        knockbackCoroutine = StartCoroutine(KnockbackRoutine(attackerPos, xForce, yForce));
    }
    private IEnumerator KnockbackRoutine(Vector3 attackerPos, float xForce, float yForce) {
        // 若是第一次被擊飛，記錄地面高度
        if (float.IsNaN(groundY)) groundY = transform.position.y;

        isKnocked = true;

        float dir = (transform.position.x - attackerPos.x) >= 0 ? 1f : -1f;
        float baseY = groundY;
        Vector3 bottomGroundPos = bottomCollider.transform.position;

        // ======= 物理參數 =======
        float verticalVelocity = yForce;     // 初始向上速度
        float gravity = gravityScale;        // 重力大小（建議 9.8 ~ 20）
        float horizontalVelocity = xForce * dir; // 水平速度（不受重力影響）

        // ======= 迴圈：持續模擬直到角色落回地面 =======
        while (true)
        {
            float deltaTime = Time.deltaTime;

            // 垂直速度逐漸被重力拉低
            verticalVelocity -= gravity * deltaTime;

            // 位移應用
            transform.position += new Vector3(
                horizontalVelocity * deltaTime,
                verticalVelocity * deltaTime,
                0
            );

            // 底部保持貼地（Y 固定，但 X 跟著走）
            bottomCollider.transform.position = new Vector3(
                transform.position.x,
                bottomGroundPos.y,
                bottomCollider.transform.position.z
            );

            // 偵測是否回到地面以下（表示落地）
            if (transform.position.y <= baseY && verticalVelocity < 0)
                break;

            yield return null;
        }

        // ======= 落地校正 =======
        transform.position = new Vector3(transform.position.x, baseY, transform.position.z);
        bottomCollider.transform.localPosition = Vector3.zero;

        isKnocked = false;
        groundY = float.NaN;
    }


    // Gizmo 顯示底部碰撞範圍
    private void OnDrawGizmosSelected() {
        if (bottomCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bottomCollider.bounds.center, bottomCollider.bounds.size);
        }
    }
}
