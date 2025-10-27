using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PhysicConfig", menuName = "GameData/PhysicConfig")]
public class PhysicConfig : ScriptableObject
{
    public float GravityScale = 15f;     //���O�j�סA�i�b Inspector �վ�
    public float BottomYThreshold = 0.5f;             //y�b�`�קP�w�H��
}