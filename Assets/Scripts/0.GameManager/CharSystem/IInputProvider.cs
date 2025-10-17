using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IInputProvider
{
    void SetIntentMove(MoveComponent moveComponent,Vector2? direction = null, Vector2? targetPosition = null, Transform targetTransform = null);
    void SetIntentSkill(SkillComponent skillComponent,int slotIndex, Vector2? targetPosition = null, Transform targetTransform = null);
}