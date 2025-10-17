using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTargetType
{
    [InspectorName("���V��")] Point,   // �Ҧp�G�a���١B����y��a�O
    [InspectorName("���w��")] Target   // �Ҧp�G��w�ĤH�g��
}

[System.Serializable]
public class SkillTemplate
{
    public StatsData StatsData;
    public VisualData VisualData;
    public float Cooldown;
    public SkillTargetType TargetType;
}

