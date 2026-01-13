using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    public int Power;
    public float MoveSpeed;
    public float VerticalMoveSpeed;

    public float KnockbackPower = 1f;
    public float FloatPower = 0f;
    public float Weight;

    public StatsData() {
        Power = 0;
        MoveSpeed = 0f;
        VerticalMoveSpeed = 0f;
        KnockbackPower = 0f;
        FloatPower = 0f;
        Weight = 0f;
    }

    public StatsData(StatsData other) {
        Power = other.Power;
        MoveSpeed = other.MoveSpeed;
        VerticalMoveSpeed= other.VerticalMoveSpeed;

        KnockbackPower = other.KnockbackPower;
        FloatPower = other.FloatPower;
        Weight = other.Weight;
    }
}
