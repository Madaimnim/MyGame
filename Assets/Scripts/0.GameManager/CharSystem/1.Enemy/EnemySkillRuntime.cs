using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySkillRuntime: ISkillRuntime
{
    public StatsData StatsData { get; private set; }
    public VisualData VisualData { get; private set; }
    public float Cooldown { get; private set; }
    public SkillTargetType SkillTargetType { get; private set; }
    public SkillDetectStrategyBase DetectStrategy { get; set; }

    public EnemySkillRuntime(SkillTemplate template){
        //²L«þ¨©
        StatsData = template.StatsData;
        VisualData = template.VisualData;
        Cooldown = template.Cooldown;
        SkillTargetType = template.TargetType;
        DetectStrategy = template.DetectType switch {
            SkillDetectType.Circle => new Circle_DetectStrategy(template.DetectRadius),
            //SkillDetectType.Cone => new SkillDetectStrategy_Cone(self, template.ConeRadius, template.DetectAngle),
            //SkillDetectType.GlobalClosest => new SkillDetectStrategy_GlobalClosest(self),
            _ => null
        };
    }
}