using UnityEngine;
using System.Collections.Generic;

public class TargetDetector : MonoBehaviour
{
    #region 公開變數

    #endregion

    #region 私有變數
    public LayerMask targetLayers; //支援多選 Layer
    public bool hasTarget=false;
    public Transform targetTransform;
    #endregion

    #region Awake()方法
    private void Awake() { }
    #endregion

    #region OnTriggerEnter2D(Collider2D collision)
    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            hasTarget = true;
            targetTransform = collision.transform;
        }
    }
    #endregion
    #region OnTriggerExit2D (Collider2D collision)
    private void OnTriggerExit2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            hasTarget = false;
        }
    }
    #endregion
}
