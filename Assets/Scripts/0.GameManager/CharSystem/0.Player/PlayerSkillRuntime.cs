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
    public SkillDetectorBase Detector { get; set; }

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

        Detector = template.DetectorType switch {
            SkillDetectorType.Circle => new Circle_Detector(template.DetectRadius),
            SkillDetectorType.Box => new Box_Detector(template.RangeX, template.RangeY),
            //SkillDetectType.GlobalClosest => new SkillDetectStrategy_GlobalClosest(self),
            _ => null
        };

        //Runtime
        SkillLevel = 1;
        SkillUsageCount = 0;
        NextLevelCount = 10;
    }
}