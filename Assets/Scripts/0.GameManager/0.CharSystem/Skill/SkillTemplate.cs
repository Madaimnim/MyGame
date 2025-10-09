using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTargetType
{
    [InspectorName("指向技")] Point,   // 例如：地裂斬、放火球到地板
    [InspectorName("指定技")] Target   // 例如：鎖定敵人射擊
}

[System.Serializable]
public class SkillTemplate
{
    public StatsData StatsData;
    public VisualData VisualData;
    public float Cooldown;
    public SkillTargetType TargetType;
}

