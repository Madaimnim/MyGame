using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class StateComponent {
    public bool CanMove => !IsDead && !IsAttackingIntent && !IsControlLocked && !IsKnocked;



    public bool IsDead { get; private set; } = false;
    public bool IsKnocked { get; private set; }=false;
    public bool IsGrounded { get; private set; }
    public bool IsAttackingIntent { get; private set; } = false;
    public bool IsMoving { get; private set; } = false;
    public bool IsPlayingAttackAnimation { get; private set; } = false;
    public bool IsControlLocked { get; private set; } = false;
    public StateComponent() {}


    public void SetIsDead(bool value) => IsDead = value;
    public void SetIsKnocked(bool value) => IsKnocked = value;
    public void SetIsGrounded(bool value) => IsGrounded = value;
    public void SetIsAttackingIntent(bool value) => IsAttackingIntent = value;
    public void SetIsMoving(bool value) => IsMoving = value;
    public void SetIsPlayingAttackAnimation(bool value) => IsPlayingAttackAnimation = value;
    public void SetIsControlLocked(bool value) => IsControlLocked = value;
}
