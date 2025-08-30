using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]

public class Test01 : MonoBehaviour
{
    public int b = 0;

    [Header("膀セ把计")]
    [Tooltip("κだゑ0~100")]
    [Range(0f, 100f)] public float skillDamage = 0f;

    [Tooltip("反计ぃ璽")]
    [Min(0f)] public float destroyDelay = 0f;

    [Header("ヘ夹瓜糷")]
    public LayerMask targetLayers;

    void Awake() {


    }
}
