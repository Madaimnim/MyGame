using UnityEngine;

public class ShowIfDetectTypeAttribute : PropertyAttribute {
    public SkillDetectType ExpectedType;

    public ShowIfDetectTypeAttribute(SkillDetectType type) {
        ExpectedType = type;
    }
}

