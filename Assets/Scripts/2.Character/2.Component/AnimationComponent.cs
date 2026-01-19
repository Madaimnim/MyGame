using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class AnimationComponent
{
    private StateComponent _stateComponent;
    private Animator _ani;
    private Transform _transform;
    private Rigidbody2D _rb;


    public AnimationComponent(Animator ani,Transform transform,Rigidbody2D rb,StateComponent stateComponent) {
        _ani = ani ?? throw new ArgumentNullException(nameof(ani), "Animator missing on prefab");
        _transform = transform;
        _rb = rb;
        _stateComponent= stateComponent;
    }


    public void PlaySkill(int skillId)=> TryPlay($"Skill{skillId}");

    public void PlayBaseAttack() => TryPlay("BaseAttack");
    public void PlayIdle() => TryPlay("Idle");
    public void PlayMove() => TryPlay("Move");
    public void PlayDie() => TryPlay("Die");
    public void PlayHurt() => TryPlay("Hurt");
    public void PlayRecover() => TryPlay("Recover");

    public void Play(string name) => _ani.Play(Animator.StringToHash(name));


    private bool TryPlay(string stateName, int layer = 0) {

        int stateNameHash = Animator.StringToHash(stateName);
        if (!_ani.HasState(layer, stateNameHash)) return false;
        _ani.Play(stateNameHash, layer);

        return true;
    }

    public void SetParameterBool(string parameter,bool value) {
        _ani.SetBool(parameter, value);
    }


}
