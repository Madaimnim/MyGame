using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    public int Id;
    public string Name;
    public int MaxHp;
    public int AttackPower;
    public float MoveSpeed;
    public float KnockbackPower;
    public float KnockbackResistance;

    public int SkillSlotCount;

    public StatsData(StatsData other) {
        Id = other.Id;
        Name = other.Name;
        MaxHp = other.MaxHp;
        AttackPower = other.AttackPower;
        MoveSpeed = other.MoveSpeed;
        KnockbackPower = other.KnockbackPower;
        KnockbackResistance = other.KnockbackResistance;
        SkillSlotCount = other.SkillSlotCount;
    }
}
