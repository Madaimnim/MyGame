﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTargetType
{
    [InspectorName("指向技")] Point,   // 例如：地裂斬、放火球到地板
    [InspectorName("指定技")] Target   // 例如：鎖定敵人射擊
}
public enum SkillDetectType {
    [InspectorName("圓形範圍")] Circle,
    [InspectorName("矩形範圍")] Box,
    [InspectorName("全場最近敵人")] GlobalClosest,
}

[System.Serializable]
public class SkillTemplate
{
    public StatsData StatsData;
    public VisualData VisualData;
    public float Cooldown;
    public SkillTargetType TargetType;
    public SkillDetectType DetectType;

    // 這三個欄位會被屬性控制是否顯示
    [ShowIfDetectType(SkillDetectType.Circle)]
    public float DetectRadius;

    [ShowIfDetectType(SkillDetectType.Box)]
    public float RangeX;
    [ShowIfDetectType(SkillDetectType.Box)]
    public float RangeY;
}

