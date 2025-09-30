using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySkillRuntime
{
    public StatsData StatsData;
    public VisualData VisualData;
    public float Cooldown;

    public EnemySkillRuntime(SkillTemplate template){
        StatsData = template.StatsData;
        Cooldown = template.Cooldown;
    }
}