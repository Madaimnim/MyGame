using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private int baseSortingOrder = 1000; // 基礎排序值，確保不會變成負數
    private float lastYPosition = 0f; // 記錄上一次的 Y 座標

    #region Awake()方法
    void Awake() {
        UpdateSortingOrder();
    }
    #endregion
    #region Start()方法
    void Start() {
        UpdateSortingOrder();
    }
    #endregion
    #region LateUpdate()方法
    void LateUpdate() {
        if (Mathf.Abs(transform.position.y - lastYPosition) > 0.01f)    //當 Y 座標變化才更新Order in Layer，提升效能
        {
            UpdateSortingOrder();
        }
    }
    #endregion

    #region 私有UpdateSortingOrder()方法
    private void UpdateSortingOrder() {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = baseSortingOrder - Mathf.RoundToInt(transform.position.y * 100);
        }
        lastYPosition = transform.position.y;
    }
    #endregion
}
