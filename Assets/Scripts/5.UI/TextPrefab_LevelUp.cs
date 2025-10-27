using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPrefab_LevelUp : MonoBehaviour
{
    public RectTransform rectTransform;
    void LateUpdate() {
        // 取父物件的縮放
        Vector3 parentScale = transform.parent != null ? transform.parent.localScale : Vector3.one;

        // 修正自己，確保 x 永遠是正的
        rectTransform.localScale = new Vector3(
            parentScale.x >= 0 ? 1f : -1f,  // 如果父物件翻轉，就反轉回來
            1f, 
            1f
            );
    }
}
