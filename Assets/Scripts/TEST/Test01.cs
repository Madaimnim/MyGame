using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]

public class Test01 : MonoBehaviour
{
    public int b = 0;

    [Header("�򥻰Ѽ�")]
    [Tooltip("�ʤ���A0~100")]
    [Range(0f, 100f)] public float skillDamage = 0f;

    [Tooltip("�۷���ơA���ର�t")]
    [Min(0f)] public float destroyDelay = 0f;

    [Header("�ؼйϼh")]
    public LayerMask targetLayers;

    void Awake() {


    }
}
