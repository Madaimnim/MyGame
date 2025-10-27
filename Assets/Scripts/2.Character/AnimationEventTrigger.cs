using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTrigger : MonoBehaviour
{
    private IAnimationEventOwner _owner;

    private void Awake()
    {
        // �q�l���󩹤W����h�� Enemy
        _owner = GetComponentInParent<IAnimationEventOwner>();
        if (_owner == null)
            Debug.LogError("�䤣����hIAnimationEventOwner�C");
    }

    public void AnimationEvent_SpawnerSkill() => _owner?.AnimationEvent_SpawnerSkill();

}
