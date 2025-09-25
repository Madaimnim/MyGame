using System;
using UnityEngine;


public class CharAnimationComponent
{


    public bool IsPlayingAttackAnimation;
    private Animator _ani;

    public CharAnimationComponent(Animator ani) {
        _ani = ani ?? throw new ArgumentNullException(nameof(ani), "Animator missing on prefab");
    }

    public void PlayIdle() =>_ani.Play(Animator.StringToHash("Idle"));
    public void PlayMove() =>_ani.Play(Animator.StringToHash("Move"));
    public void PlayDie()=>_ani.Play(Animator.StringToHash("Die"));
    public void Play(string name) => _ani.Play(Animator.StringToHash(name));
}
