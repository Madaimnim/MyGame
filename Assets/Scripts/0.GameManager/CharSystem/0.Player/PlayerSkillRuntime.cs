using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkillRuntime: ISkillRuntime
{
    public StatsData StatsData { get; set; }
    public VisualData VisualData { get; set; }
    public float Cooldown { get; set; }
    public SkillTargetType SkillTargetType { get; set; }
    public SkillDetectStrategyBase DetectStrategy { get; set; }

    //Player¿W¦³
    public int SkillLevel;
    public int SkillUsageCount;
    public int NextLevelCount;

    public PlayerSkillRuntime(SkillTemplate template) {
        //²`«þ¨©
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        Cooldown = template.Cooldown;
        SkillTargetType = template.TargetType;

        DetectStrategy = template.DetectType switch {
            SkillDetectType.Circle => new Circle_DetectStrategy(template.DetectRadius),
            //SkillDetectType.Cone => new SkillDetectStrategy_Cone(self, template.ConeRadius, template.DetectAngle),
            //SkillDetectType.GlobalClosest => new SkillDetectStrategy_GlobalClosest(self),
            _ => null
        };

        //Runtime
        SkillLevel = 1;
        SkillUsageCount = 0;
        NextLevelCount = 10;
    }
}