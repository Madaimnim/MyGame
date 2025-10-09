using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillRuntime
{
    StatsData StatsData { get; }
    VisualData VisualData { get; }
    float Cooldown { get; }
    SkillTargetType TargetType { get; }
}