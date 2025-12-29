using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class StateComponent {
    public bool CanMove => !IsDead && !IsControlLocked && !IsKnocked && !IsSkillDashing;
    public bool CanAttack => !IsDead && !IsControlLocked && !IsKnocked && !IsPlayingAttackAnimation && IsInitialHeight;
    public bool CanRecoverHeight => !IsDead  && !IsControlLocked && !IsKnocked && !IsSkillDashing && !IsPlayingAttackAnimation && IsInitialHeight ;

    public bool IsDead { get; private set; } = false;
    public bool IsKnocked { get; private set; }=false;
    public bool IsGrounded { get; private set; }
    public bool IsInitialHeight { get; private set; } = true;
    public bool IsAttackingIntent { get; private set; } = false;
    public bool IsMoving { get; private set; } = false;
    public bool IsPlayingAttackAnimation { get; private set; } = false;
    public bool IsControlLocked { get; private set; } = false;
    public bool IsMoveAnimationOpen { get; private set; } = false;
    public bool IsInGravityFall { get; private set; } = false;

    //技能狀態
    public bool IsSkillDashing { get;private set; } = false;
    public StateComponent() {}


    public void SetIsDead(bool value) => IsDead = value;
    public void SetIsKnocked(bool value) => IsKnocked = value;
    public void SetIsGrounded(bool value) => IsGrounded = value;
    public void SetIsInitialHeight(bool value) => IsInitialHeight = value;
    public void SetIsAttackingIntent(bool value) => IsAttackingIntent = value;
    public void SetIsMoving(bool value) => IsMoving = value;
    public void SetIsPlayingAttackAnimation(bool value) => IsPlayingAttackAnimation = value;
    public void SetIsControlLocked(bool value) => IsControlLocked = value;
    public void SetIsMoveAnimationOpen(bool value) => IsMoveAnimationOpen = value;
    public void SetIsInGravityFall(bool value) => IsInGravityFall = value;
    //技能狀態
    public void SetIsSkillDashing(bool value) => IsSkillDashing = value;
}
