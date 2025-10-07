using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IInputProvider
{
    void SetIntentMoveDirection();
    void SetIntentSkillSlot();
    void SetIntentTargetPosition();
}