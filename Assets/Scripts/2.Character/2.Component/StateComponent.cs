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
    #region Can、Is狀態
    public bool CanAction => !IsDead && !IsHurt;

    public bool CanMove => CanAction && !IsKnocked && !IsCastingSkill &&! IsCastingSkill && !IsBaseAttacking&&!IsSkillDashing;
    public bool CanBaseAttack => CanAction && !IsKnocked  && IsCanAttackHeight;
    public bool CanCastSkill => CanAction && !IsKnocked  && IsCanAttackHeight;   
    public bool ShoudApplyGravity => !IsGrounded && !IsAntiGravity;


    public bool IsHurt {  get; private set; } = false;
    public bool IsDead { get; private set; } = false;
    public bool IsMoving { get; private set; } = false;
    public bool IsBaseAttacking { get; private set; } = false;          //普通攻擊模式中
    public bool IsCastingSkill { get; private set; } = false;           // 技能施放中

    public bool IsKnocked { get; private set; }=false;


    public bool IsCanAttackHeight { get; private set; } = true;         //由HeightComponent統一更新
    public bool IsGrounded { get; private set; } = false;               //由HeightComponent統一更新
    public bool IsAntiGravity { get; private set; } = false;
    public bool IsRecoveringHeight { get; private set; } = false;       


    public bool IsSkillDashing { get;private set; } = false;
    #endregion


    private DebugContext _context = DebugContext.None;
    private int _id = -1;
    private MonoBehaviour _owner;
    private Coroutine _hurtCoroutine;

    public StateComponent(MonoBehaviour owner,DebugContext context = DebugContext.None, int id=-1) {
        _context = context;
        _id = id; 
        _owner= owner;
    }


    #region SetIs區
    public void SetIsHurt(bool value) => IsHurt = value;
    public void SetIsDead(bool value) => IsDead = value; 
    public void SetIsMoving(bool value) => IsMoving = value;
    public void SetIsBaseAttacking(bool value) => IsBaseAttacking = value;
    public void SetIsCastingSkill(bool value) => IsCastingSkill = value;


    public void SetIsKnocked(bool value) => IsKnocked = value;


    public void SetIsCanAttackHeight(bool value) => IsCanAttackHeight = value;  //由HeightComponent統一更新
    public void SetIsGrounded(bool value) => IsGrounded = value;                //由HeightComponent統一更新
    public void SetIsAntiGravity(bool value) => IsAntiGravity = value;
    public void SetIsRecoveringHeight(bool value) =>IsRecoveringHeight = value;

    public void SetIsSkillDashing(bool value) => IsSkillDashing = value;
    #endregion
    public void ResetState() {
        IsHurt = false;
        IsDead = false;
        IsMoving = false;
        IsBaseAttacking = false;
        IsCastingSkill = false;

        IsKnocked = false;

        IsAntiGravity = false;
        IsRecoveringHeight = false;

        IsSkillDashing = false;
    }


    #region Debug區
    public void DebugState() {                      //Debug搭配EnemyScreenDebug用、EnemyScreenDebug用
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
            { "IsAntiGravity", IsAntiGravity},
            {"IsCanAttackHeight", IsCanAttackHeight },

            { "IsSkillDashing", IsSkillDashing },

        };
    }
    #endregion


}
