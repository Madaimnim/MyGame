using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAutoDestroy : MonoBehaviour
{
    private Animator _animator;
    private void Start() {
        _animator = GetComponent<Animator>();
        float clipLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, clipLength);
    }
}
