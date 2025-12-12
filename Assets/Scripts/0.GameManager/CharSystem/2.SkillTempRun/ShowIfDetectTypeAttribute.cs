using UnityEngine;

public class ShowIfDetectTypeAttribute : PropertyAttribute {
    public SkillDetectorType ExpectedType;

    public ShowIfDetectTypeAttribute(SkillDetectorType type) {
        ExpectedType = type;
    }
}

