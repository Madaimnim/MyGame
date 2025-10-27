using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PhysicConfig", menuName = "GameData/PhysicConfig")]
public class PhysicConfig : ScriptableObject
{
    public float GravityScale = 15f;     //重力強度，可在 Inspector 調整
    public float BottomYThreshold = 0.5f;             //y軸深度判定閾值
}