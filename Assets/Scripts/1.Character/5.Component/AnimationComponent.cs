using System;
using UnityEngine;


public class AnimationComponent
{
    public bool IsPlayingAttackAnimation;
    public bool IsMoving => _rb.velocity != Vector2.zero;

    private Animator _ani;
    private Transform _transform;
    private Rigidbody2D _rb;

    public AnimationComponent(Animator ani,Transform transform,Rigidbody2D rb) {
        _ani = ani ?? throw new ArgumentNullException(nameof(ani), "Animator missing on prefab");
        _transform = transform;
        _rb = rb;
    }


    public void PlayAttackAnimation(ISkillRuntime skill, Vector3 targetPosition) {
        // 翻轉角色朝向
        bool isTargetOnLeft = targetPosition.x < _transform.position.x;
        _transform.localScale = new Vector3(
            isTargetOnLeft ? -Mathf.Abs(_transform.localScale.x) : Mathf.Abs(_transform.localScale.x),
            _transform.localScale.y,
            _transform.localScale.z
        );

        // 播放動畫
        if (IsMoving)
            Play($"MoveSkill{skill.StatsData.Id}");
        else
            Play($"Skill{skill.StatsData.Id}");
    }


    public void PlayIdle() =>_ani.Play(Animator.StringToHash("Idle"));
    public void PlayMove() =>_ani.Play(Animator.StringToHash("Move"));
    public void PlayDie()=>_ani.Play(Animator.StringToHash("Die"));
    public void Play(string name) => _ani.Play(Animator.StringToHash(name));

    

}
