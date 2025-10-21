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

    public Vector2 KnockbackForce;
    public float Weight;


    public StatsData(StatsData other) {
        Id = other.Id;
        Name = other.Name;
        Level = other.Level;

        Power = other.Power;
        MoveSpeed = other.MoveSpeed;

        KnockbackForce= other.KnockbackForce;
        Weight = other.Weight;
    }
}
