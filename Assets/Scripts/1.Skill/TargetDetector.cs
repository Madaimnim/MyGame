using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

public class TargetDetector : MonoBehaviour
{
    public TargetPriorityType targetPriorityType = TargetPriorityType.Closest;
    public LayerMask targetLayers;
    [HideInInspector] public bool hasTarget = false;
    [HideInInspector] public Transform targetTransform;
    private List<Transform> targetsList = new List<Transform>();

    private float targetUpdateCooldown = 0f; // 更新間隔計時器
    private float targetUpdateInterval = 0.2f; // 每 0.2 秒更新目標

    public enum TargetPriorityType
    {
        [InspectorName("最近敵人")] Closest,
        [InspectorName("x軸最小敵人")] LeftMost
    }

    private void Update() {
        UpdateTargetInSecond(targetUpdateInterval);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            if (!targetsList.Contains(collision.transform)) // 防止重複添加同一目標
            {
                targetsList.Add(collision.transform);
                UpdateTargetTransform();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            targetsList.Remove(collision.transform);

            //如果當前targetTransform離開範圍，強制更新targetTransform = null，避免攻擊到範圍外目標
            if (targetTransform == collision.transform)
            {
                targetTransform = null;
                hasTarget = false;
                UpdateTargetTransform();
            }
        }
    }


    #region UpdateTargetInSecond(float targetUpdateInterval)
    private void UpdateTargetInSecond(float targetUpdateInterval) {
        targetUpdateCooldown -= Time.deltaTime;
        if (targetUpdateCooldown <= 0f)
        {
            UpdateTargetTransform();
            targetUpdateCooldown = targetUpdateInterval; //確保時間間隔穩定
        }
    }
    private void UpdateTargetTransform() {
        targetsList.RemoveAll(t => t == null); // 清除已銷毀的目標

        if (targetsList.Count == 0)
        {
            hasTarget = false;
            targetTransform = null;
            return;
        }

        Transform bestTarget = null;
        switch (targetPriorityType)
        {
            #region TargetPriorityType.Closest
            case TargetPriorityType.Closest:
                float minDistance = float.MaxValue;
                foreach (Transform t in targetsList)
                {
                    float distance = Vector2.Distance(transform.position, t.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestTarget = t;
                    }
                }
                break;
            #endregion

            #region TargetPriorityType.LeftMost
            case TargetPriorityType.LeftMost:
                float minX = float.MaxValue;
                foreach (Transform t in targetsList)
                {
                    if (t.position.x < minX)
                    {
                        minX = t.position.x;
                        bestTarget = t;
                    }
                }
                break;
                #endregion
        }

        targetTransform = bestTarget;
        hasTarget = (targetTransform != null);
    }
    #endregion

    //提供外部可執行方法，變更策略
    #region SetTargetPriority(TargetPriorityType newPriority)
    public void SetTargetPriority(TargetPriorityType newPriority) {
        if (targetPriorityType != newPriority)
        {
            targetPriorityType = newPriority;
            UpdateTargetTransform(); //改變優先級時立即更新目標
        }
    }
    #endregion
}
