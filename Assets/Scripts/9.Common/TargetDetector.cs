using UnityEngine;
using System.Collections.Generic;

public class TargetDetector : MonoBehaviour
{
    #region 公開變數

    #endregion

    #region 私有變數
    private LayerMask targetLayers; // ✅ 支援多選 Layer
    private bool hasTarget;
    #endregion

    #region Awake()方法
    private void Awake() { }
    #endregion

    #region OnTriggerEnter2D(Collider2D collision)
    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            hasTarget = true; // ✅ 設為 true，表示偵測到目標
        }
    }
    #endregion
    #region OnTriggerExit2D (Collider2D collision)
    private void OnTriggerExit2D(Collider2D collision) {
        // ✅ 只檢測 Layer，不用處理 IDamageable
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            hasTarget = false;
        }
    }
    #endregion

    #region 設定攻擊目標 LayerMask
    public void SetTargetLayers(LayerMask layers) {
        targetLayers = layers; // ✅ 傳遞 LayerMask
    }
    #endregion
}
