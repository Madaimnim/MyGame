using System;
using UnityEngine;


public class AnimationComponent
{
    public bool IsPlayingAttackAnimation;
    public bool IsMoving => _moveComponent.IsMoving;

    private Animator _ani;
    private Transform _transform;
    private Rigidbody2D _rb;
    private MoveComponent _moveComponent;

    public AnimationComponent(Animator ani,Transform transform,Rigidbody2D rb, MoveComponent moveComponent) {
        _ani = ani ?? throw new ArgumentNullException(nameof(ani), "Animator missing on prefab");
        _transform = transform;
        _rb = rb;
        _moveComponent = moveComponent;
    }


    public void PlayAttackAnimation(int skillId) {
        if (IsMoving)
        {
            //Debug.Log($"6");
            Play($"MoveSkill{skillId}");
        }
        else
        {
            //Debug.Log($"7");
            Play($"Skill{skillId}");
        }

    }

    public void FaceDirection(Vector2 direction) {
        if (direction.x == 0) return; 

        float absScaleX = Mathf.Abs(_transform.localScale.x);
        _transform.localScale = new Vector3(
            direction.x < 0 ? -absScaleX : absScaleX,
            _transform.localScale.y,
            _transform.localScale.z
        );
    }


    public void PlayIdle() =>_ani.Play(Animator.StringToHash("Idle"));
    public void PlayMove() =>_ani.Play(Animator.StringToHash("Move"));
    public void PlayDie()=>_ani.Play(Animator.StringToHash("Die"));
    public void Play(string name) => _ani.Play(Animator.StringToHash(name));

    

}
