using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSkillRuntime: ISkillRuntime
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
    public float EnergyCost { get; set; }                 // 玩家用
    public float EnergyGain { get; set; }                 // 玩家用
    public float Cooldown { get; set; }
    public float DestroyDelay { get; set; }
    public float OnHitDestroyDelay { get; set; }
    public float SkillDashMultiplier { get; set; }
    public float SkillDashDuration { get; set; }
    public float SkillDashPrepareDuration { get; set; }
    public float SkillDashVerticalVelocity { get; set; }

    public SkillDetectorBase Detector { get; set; }

    //Player獨有
    public int SkillLevel;
    public int SkillUsageCount;
    public int NextLevelCount;
    public PlayerSkillRuntime EnhancedSkillRuntime;

    public PlayerSkillRuntime(SkillTemplate template) {
        //深拷貝
        SkillReleaseType = template.SkillReleaseType;
        SkillLifetimeType =template.SkillLifetimeType;
        SkillVisualFromType = template.SkillVisualFromType;
        SkillMoveType = template.SkillMoveType;
        OnHitType = template.OnHitType;
        HitEffectPositionType = template.HitEffectPositionType;
        FacingDirection = template.FacingDirection;
        TargetLayers = template.TargetLayers;

        Id = template.Id;
        Name = template.Name;
        StatsData = new StatsData(template.StatsData);
        VisualData = new VisualData(template.VisualData);
        canRotate = template.canRotate;
        SkillCostType=SkillCostType.Energy;
        EnergyCost = template.EnergyCost;
        EnergyGain = template.EnergyGain;
        Cooldown = template.Cooldown;
        DestroyDelay = template.DestroyDelay;
        OnHitDestroyDelay = template.OnHitDestroyDelay;
        SkillDashMultiplier = template.SkillDashMultiplier;
        SkillDashDuration = template.SkillDashDuration;
        SkillDashPrepareDuration = template.SkillDashPrepareDuration;
        SkillDashVerticalVelocity = template.SkillDashVerticalVelocity;

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