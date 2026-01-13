using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SkillReleaseType
{
    [InspectorName("指向技")] Towerd,   // 例如：地裂斬、放火球到地板
    [InspectorName("指定技")] Target   // 例如：鎖定敵人射擊
}
public enum SkillLifetimeType {
    [InspectorName("設定持續時間")] TimeLimit,   // 用 DestroyDelay
    [InspectorName("動畫一次")] AnimationOnce,  // 動畫播完就消失
}
public enum SkillVisualFromType {
    [InspectorName("來自技能物件的攻擊")] FromSkill,
    [InspectorName("來自角色的攻擊")] FromChar
}

public enum SkillMoveType {
    [InspectorName("原地生成")] Station,
    [InspectorName("追蹤目標")] Homing,
    [InspectorName("直線朝目標發射")] Toward,
    [InspectorName("拋物線朝目標發射")] ParabolaToward,
    [InspectorName("直線飛行")] Straight,
    [InspectorName("生成於目標位置")] SpawnAtTarget,
    [InspectorName("附在角色")] AttackToOwner
}
public enum OnHitType {
    [InspectorName("沒反應")] Nothing,                      //沒反應
    [InspectorName("命中消失")] Disappear,                  //命中即消失
    [InspectorName("命中後停留爆炸")] Explode,              //命中後停留爆炸
}
public enum HitEffectPositionType {
    [InspectorName("最近點")] ClosestPoint,
    [InspectorName("中心")] TargetCenter,
    [InspectorName("上半部")] TargetUpper
}
public enum FacingDirection {
    [InspectorName("美術朝左")] Left,
    [InspectorName("美術朝右")] Right
}
public enum SkillDetectorType {
    [InspectorName("圓形範圍")] Circle,
    [InspectorName("矩形範圍")] Box,
    [InspectorName("全場最近敵人")] GlobalClosest,
}

[System.Serializable]
public class SkillTemplate {
    public int Id;
    public string Name;
    public StatsData StatsData;
    public VisualData VisualData;
    [Header("發動方式")]
    public SkillReleaseType SkillReleaseType;

    [Header("技能類型")]
    public SkillLifetimeType SkillLifetimeType;
    public SkillVisualFromType SkillVisualFromType;
    public SkillMoveType SkillMoveType;
    public OnHitType OnHitType;
    public HitEffectPositionType HitEffectPositionType;
    public FacingDirection FacingDirection = FacingDirection.Right;
    public LayerMask TargetLayers;


    public bool canRotate;
    public float Cooldown;
    public float DestroyDelay = 0f;
    public float OnHitDestroyDelay = 0f;
    public float SkillDashMultiplier = 1f;
    public float SkillDashDuration = 0f;
    public float SkillDashPrepareDuration = 0f;
    public float SkillDashVerticalVelocity = 0f;

    [Header("技能偵測器")]
    public SkillDetectorType DetectorType;
    [ShowIfDetectType(SkillDetectorType.Circle)]
    public float DetectRadius;

    [ShowIfDetectType(SkillDetectorType.Box)]
    public float RangeX;
    [ShowIfDetectType(SkillDetectorType.Box)]
    public float RangeY;

}

