using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkillRuntime
{
    public StatsData StatsData;
    public VisualData VisualData;
    public float Cooldown;

    //Player¿W¦³
    public int SkillLevel;
    public int SkillUsageCount;
    public int NextLevelCount;

    public PlayerSkillRuntime(SkillTemplate template) {
        //²`«þ¨©
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);

        //Runtime
        SkillLevel = 1;
        SkillUsageCount = 0;
        NextLevelCount = 10;
    }
}