using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableHide : MonoBehaviour
{
    public void OnEnable() {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
    }
}
