using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillInput
{
    Vector3 GetTargetPosition(int slotIndex, SkillSlot slot);
}

public class PlayerMouseInput : ISkillInput
{
    public Vector3 GetTargetPosition(int slotIndex, SkillSlot slot) {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}

public class AISkillInput : ISkillInput
{
    public Vector3 GetTargetPosition(int slotIndex, SkillSlot slot) {
        var detector = slot.Detector;
        return detector != null && detector.HasTarget ? detector.TargetTransform.position : Vector3.zero;
    }
}
