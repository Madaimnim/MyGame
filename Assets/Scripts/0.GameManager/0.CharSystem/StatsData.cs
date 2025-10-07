using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    public int Id;
    public string Name;
    public int Level;

    public int Power;
    public float MoveSpeed;

    public float KnockbackPower;
    public float KnockbackResistance;


    public StatsData(StatsData other) {
        Id = other.Id;
        Name = other.Name;
        Level = other.Level;

        Power = other.Power;
        MoveSpeed = other.MoveSpeed;

        KnockbackPower = other.KnockbackPower;
        KnockbackResistance = other.KnockbackResistance;
    }
}
