using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPrefab_LevelUp : MonoBehaviour
{
    public RectTransform rectTransform;
    void LateUpdate() {
        // ���������Y��
        Vector3 parentScale = transform.parent != null ? transform.parent.localScale : Vector3.one;

        // �ץ��ۤv�A�T�O x �û��O����
        rectTransform.localScale = new Vector3(
            parentScale.x >= 0 ? 1f : -1f,  // �p�G������½��A�N����^��
            1f, 
            1f
            );
    }
}
