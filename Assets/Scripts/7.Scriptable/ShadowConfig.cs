using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShadowConfig", menuName = "GameConfig/ShadowConfig")]
public class ShadowConfig : ScriptableObject
{
    [Header("影子透明度範圍")]
    [Range(0f, 1f)]
    public float MaxAlpha = 0.35f;
    [Range(0f, 1f)]
    public float MinAlpha = 0.05f;


    [Header("太陽高度(越高值越小")]
    public float SunLengthMultiplier = 1.3f;

    [Header("高度造成的影子拉長")]
    public float MaxHeightStretch = 1.5f;

    [Header("隨腳色高度變化比例")]
    public float ShadowHeightRatio = 0.5f;
    [Header("最大有影子高度")]
    public float FadeOutHeight = 2f;
}