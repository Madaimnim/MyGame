using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillBase 
{
    public int SkillId;
    public string SkillName;
    public int SkillPower;
    public int SkillLevel;
    public float SkillCooldown;
    public float KnockbackForce;
    public GameObject SkillPrefab;
    public GameObject TargetDetectPrefab;
}
