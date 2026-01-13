using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsComponent
{
    public StatsData BaseStats;     // 角色基礎
    public StatsData BonusStats;    // 等級 / 裝備 / Buff
    public StatsData FinalStats;    // 真正給 Skill / Move / Hit 用

    public StatsComponent(StatsData baseStats)
    {
        BaseStats = baseStats;
        BonusStats = new StatsData();
        FinalStats = new StatsData();
        RecalculateFinalStats();
    }

    public void RecalculateFinalStats() {
        
        FinalStats.Power =BaseStats.Power + BonusStats.Power;
        FinalStats.MoveSpeed =BaseStats.MoveSpeed + BonusStats.MoveSpeed;
        FinalStats.VerticalMoveSpeed =BaseStats.VerticalMoveSpeed + BonusStats.VerticalMoveSpeed;
        FinalStats.KnockbackPower =BaseStats.KnockbackPower + BonusStats.KnockbackPower;
        FinalStats.FloatPower =BaseStats.FloatPower + BonusStats.FloatPower;
        FinalStats.Weight =BaseStats.Weight + BonusStats.Weight;
        //Debug.Log($"重新計算最終屬性FinalStats.Power{FinalStats.Power}=Base{BaseStats.Power}+Bonus{BonusStats.Power}");
    }
}
