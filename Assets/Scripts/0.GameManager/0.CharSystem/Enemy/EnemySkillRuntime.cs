using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySkillRuntime: ISkillRuntime
{
    public StatsData StatsData { get; private set; }
    public VisualData VisualData { get; private set; }
    public float Cooldown { get; private set; }

    public EnemySkillRuntime(SkillTemplate template){
        //²L«þ¨©
        StatsData = template.StatsData;
        VisualData = template.VisualData;
        Cooldown = template.Cooldown;
    }
}