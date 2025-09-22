using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SkillBase 
{
    public int SkillId;
    public string SkillName;
    public int SkillPower;
    public float SkillCooldown;
    public float KnockbackForce;
    public GameObject SkillPrefab;
    public GameObject TargetDetectPrefab;
}
