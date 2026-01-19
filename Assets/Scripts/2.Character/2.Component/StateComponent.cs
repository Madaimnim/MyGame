using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Cryptography;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
public enum DebugContext {
    None,
    Player,
    Enemy
}
public class StateComponent {
    public bool CanMove => !IsDead && !IsControlLocked && !IsKnocked && !IsCastingSkill &&! IsCastingSkill && !IsBaseAttacking&&!IsSkillDashing;
    public bool CanBaseAttack => !IsDead && !IsControlLocked && !IsKnocked  && IsInitialHeight;
    public bool CanCastSkill => !IsDead && !IsControlLocked && !IsKnocked  && IsInitialHeight;
    public bool CanRecoverHeight => !IsDead  && !IsControlLocked && !IsKnocked && !IsSkillDashing && !IsCastingSkill && IsInitialHeight ;

    public bool IsDead { get; private set; } = false;
    public bool IsMoving { get; private set; } = false;
    public bool IsBaseAttacking { get; private set; } = false;        // 普通攻擊模式中
    public bool IsCastingSkill { get; private set; } = false;        // 技能施放中

    public bool IsKnocked { get; private set; }=false;
    public bool IsGrounded { get; private set; } = false;
    public bool IsInitialHeight { get; private set; } = true;
    public bool IsSkillRecoveryActionLock { get; private set; } = false;

    public bool IsControlLocked { get; private set; } = false;
    public bool IsMoveAnimationOpen { get; private set; } = false;
    public bool IsInGravityFall { get; private set; } = false;
    public bool IsSkillDashing { get;private set; } = false;

    // 身份（建構時注入）
    private DebugContext _context = DebugContext.None;
    private int _id = -1;

    public StateComponent(DebugContext context = DebugContext.None, int id=-1) {
        _context = context;
        _id = id;
    }

    public void ResetState() {
        IsDead = false;
        IsMoving = false;
        IsBaseAttacking = false;
        IsCastingSkill = false;

        IsKnocked = false;
        IsGrounded = false;
        IsInitialHeight = true;
        IsSkillRecoveryActionLock = false;

        IsControlLocked = false;
        IsMoveAnimationOpen = false;
        IsInGravityFall = false;
        IsSkillDashing = false;
    }

    public void SetIsDead(bool value) => IsDead = value; 
    public void SetIsMoving(bool value) => IsMoving = value;
    public void SetIsBaseAttacking(bool value) => IsBaseAttacking = value;
    
    public void SetIsCastingSkill(bool value) => IsCastingSkill = value;

    public void SetIsKnocked(bool value) => IsKnocked = value;
    public void SetIsGrounded(bool value) => IsGrounded = value;
    public void SetIsInitialHeight(bool value) => IsInitialHeight = value;
    public void SetIsSkillRecoveryActionLock(bool value) => IsSkillRecoveryActionLock = value;

    public void SetIsControlLocked(bool value) => IsControlLocked = value;
    public void SetIsMoveAnimationOpen(bool value) => IsMoveAnimationOpen = value;
    public void SetIsInGravityFall(bool value) => IsInGravityFall = value;
    public void SetIsSkillDashing(bool value) => IsSkillDashing = value;

    //Debug搭配EnemyScreenDebug用、EnemyScreenDebug用
    public void DebugState() {
        if (_context == DebugContext.None) return;

        // 判斷目前是否該顯示
        if (_context == DebugContext.Player &&
            PlayerScreenDebug.DebugPlayerId != _id)
            return;

        if (_context == DebugContext.Enemy &&
            EnemyScreenDebug.DebugEnemyId != _id)
            return;

        string prefix = _context == DebugContext.Player
            ? $"[Player {_id}]"
            : $"[Enemy {_id}]";

        var snapshot = GetDebugSnapshot();

        foreach (var kv in snapshot) {
            string key = $"{prefix} {kv.Key}";

            if (_context == DebugContext.Player)
                PlayerScreenDebug.Set(key, kv.Value);
            else
                EnemyScreenDebug.Set(key, kv.Value);
        }
    }
    private Dictionary<string, bool> GetDebugSnapshot() {
        return new Dictionary<string, bool>
        {
            { "IsDead", IsDead },
            { "IsMoving", IsMoving },
            { "IsBaseAttacking", IsBaseAttacking },
            { "IsCastingSkill", IsCastingSkill },

            { "IsKnocked", IsKnocked },
            { "IsGrounded", IsGrounded },
            { "IsInitialHeight", IsInitialHeight },
            {"IsSkillRecoveryActionLock", IsSkillRecoveryActionLock },

            { "IsInGravityFall", IsInGravityFall },
            { "IsMoveAnimationOpen", IsMoveAnimationOpen },
            { "IsControlLocked", IsControlLocked },
            { "IsSkillDashing", IsSkillDashing },

            { "CanMove", CanMove },
            { "CanBaseAttack", CanBaseAttack },
            { "CanCastSkill", CanCastSkill },
            { "CanRecoverHeight", CanRecoverHeight },
        };
    }
}
