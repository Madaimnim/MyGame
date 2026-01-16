using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySkillRuntime: ISkillRuntime
{
    public SkillReleaseType SkillReleaseType { get; private set; }
    public SkillLifetimeType SkillLifetimeType {  get; private set; }
    public SkillVisualFromType SkillVisualFromType { get; private set; }
    public SkillMoveType SkillMoveType { get; set; }
    public OnHitType OnHitType { get; set; }
    public HitEffectPositionType HitEffectPositionType { get; set; }
    public FacingDirection FacingDirection { get; set; }
    public LayerMask TargetLayers { get; set; }

    public int Id { get; set; }
    public string Name { get; set; }
    public StatsData StatsData { get; private set; }
    public VisualData VisualData { get; private set; }
    public bool canRotate { get; set; }
    public SkillCostType SkillCostType { get; set; }
    public float EnergyCost { get; set; }
    public float EnergyGain { get; set; }
    public float Cooldown { get; set; }
    public float DestroyDelay { get; set; }
    public float OnHitDestroyDelay { get; set; }
    public float SkillDashMultiplier { get; set; }
    public float SkillDashDuration { get; set; }
    public float SkillDashPrepareDuration { get; set; }
    public float SkillDashVerticalVelocity { get; set; }

    public SkillDetectorBase Detector { get; set; }

    public EnemySkillRuntime(SkillTemplate template){
        //²L«þ¨©
        SkillReleaseType = template.SkillReleaseType;
        SkillLifetimeType= template.SkillLifetimeType;
        SkillVisualFromType = template.SkillVisualFromType;
        SkillMoveType = template.SkillMoveType;
        OnHitType= template.OnHitType;
        HitEffectPositionType= template.HitEffectPositionType;
        FacingDirection= template.FacingDirection;
        TargetLayers= template.TargetLayers;

        Id = template.Id;
        Name = template.Name;
        StatsData = template.StatsData;
        VisualData = template.VisualData;
        canRotate = template.canRotate;
        SkillCostType = SkillCostType.Cooldown;
        EnergyCost = template.EnergyCost;
        Cooldown = template.Cooldown;
        DestroyDelay= template.DestroyDelay;
        OnHitDestroyDelay= template.OnHitDestroyDelay;
        SkillDashMultiplier = template.SkillDashMultiplier;
        SkillDashDuration = template.SkillDashDuration;
        SkillDashPrepareDuration= template.SkillDashPrepareDuration;
        SkillDashVerticalVelocity = template.SkillDashVerticalVelocity;

        Detector = template.DetectorType switch {
            SkillDetectorType.Circle => new Circle_Detector(template.DetectRadius),
            //SkillDetectType.Cone => new SkillDetectStrategy_Cone(self, template.ConeRadius, template.DetectAngle),
            //SkillDetectType.GlobalClosest => new SkillDetectStrategy_GlobalClosest(self),
            _ => null
        };
    }
}