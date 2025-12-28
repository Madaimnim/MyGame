using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySkillRuntime: ISkillRuntime
{
    public SkillTargetType SkillTargetType { get; private set; }

    public SkillExecutionType SkillExecutionType { get; private set; }
    public SkillMoveType SkillMoveType { get; set; }
    public OnHitType OnHitType { get; set; }
    public HitEffectPositionType HitEffectPositionType { get; set; }
    public FacingDirection FacingDirection { get; set; }
    public LayerMask TargetLayers { get; set; }

    public StatsData StatsData { get; private set; }
    public VisualData VisualData { get; private set; }
    public bool canRotate { get; set; }
    public float Cooldown { get; set; }
    public float DestroyDelay { get; set; }
    public float OnHitDestroyDelay { get; set; }
    public float SkillDashMultiplier { get; set; }
    public float SkillDashDuration { get; set; }
    public float SkillDashVerticalVelocity { get; set; }

    public SkillDetectorBase Detector { get; set; }

    public EnemySkillRuntime(SkillTemplate template){
        //²L«þ¨©
        SkillTargetType= template.SkillTargetType;

        SkillExecutionType = template.SkillExecutionType;
        SkillMoveType = template.SkillMoveType;
        OnHitType= template.OnHitType;
        HitEffectPositionType= template.HitEffectPositionType;
        FacingDirection= template.FacingDirection;
        TargetLayers= template.TargetLayers;

        StatsData = template.StatsData;
        VisualData = template.VisualData;
        canRotate = template.canRotate;
        Cooldown = template.Cooldown;
        DestroyDelay= template.DestroyDelay;
        OnHitDestroyDelay= template.OnHitDestroyDelay;
        SkillDashMultiplier = template.SkillDashMultiplier;
        SkillDashDuration = template.SkillDashDuration;
        SkillDashVerticalVelocity= template.SkillDashVerticalVelocity;

        Detector = template.DetectorType switch {
            SkillDetectorType.Circle => new Circle_Detector(template.DetectRadius),
            //SkillDetectType.Cone => new SkillDetectStrategy_Cone(self, template.ConeRadius, template.DetectAngle),
            //SkillDetectType.GlobalClosest => new SkillDetectStrategy_GlobalClosest(self),
            _ => null
        };
    }
}