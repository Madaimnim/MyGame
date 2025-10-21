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

    public AnimationComponent(Animator ani,Transform transform,Rigidbody2D rb) {
        _ani = ani ?? throw new ArgumentNullException(nameof(ani), "Animator missing on prefab");
        _transform = transform;
        _rb = rb;
    }
    public void Initial(MoveComponent moveComponent) {
        _moveComponent = moveComponent;
    }

    public void PlayAttackAnimation(int skillId) {
        string moveSkillName = $"MoveSkill{skillId}";
        int moveSkillHash = Animator.StringToHash(moveSkillName);

        if (IsMoving)
        {
            if(_ani.HasState(0, moveSkillHash))
            {
                Debug.Log("有移動攻擊，撥放");
                Play($"MoveSkill{skillId}");
            }
            else
            {
                Debug.Log("沒有移動攻擊，撥放站立攻擊");
                Play($"Skill{skillId}");
            }

        }   
        else
        {
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
    public void PlayDie() => _ani.Play(Animator.StringToHash("Die"));
    public void Play(string name) => _ani.Play(Animator.StringToHash(name));


}
