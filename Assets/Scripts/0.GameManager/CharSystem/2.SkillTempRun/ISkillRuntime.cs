using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillRuntime
{
    SkillReleaseType SkillReleaseType { get; }
    SkillLifetimeType SkillLifetimeType { get; }
    SkillExecutionType SkillExecutionType { get; }
    SkillMoveType SkillMoveType { get; }
    OnHitType OnHitType { get; }
    HitEffectPositionType HitEffectPositionType { get; }
    FacingDirection FacingDirection { get; }
    LayerMask TargetLayers { get; }

    StatsData StatsData { get; }
    VisualData VisualData { get; }
    bool canRotate { get; }
    float Cooldown { get; }
    float DestroyDelay { get; }
    float OnHitDestroyDelay { get; }
    float SkillDashMultiplier { get; }
    float SkillDashDuration { get; }
    float SkillDashPrepareDuration{ get; }
    float SkillDashVerticalVelocity { get; }

    SkillDetectorBase Detector { get; }
}