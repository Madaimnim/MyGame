using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoundsProvider : MonoBehaviour
{
    private void Awake() {
        //Debug.Log($"[Provider] self = {gameObject.name}");
        //Debug.Log($"[Provider] 子物件數量 = {transform.childCount}");

        MoveBoundsManager.Instance.ClearBounds();

        var colliders = GetComponentsInChildren<BoxCollider2D>(includeInactive: true);
        //Debug.Log($"場景Bounds colliders數量 = {colliders.Length}");
        foreach (var col in colliders) {
            MoveBoundsManager.Instance.Register(col);
        }
    }

}
