using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTrigger : MonoBehaviour
{
    private IAnimationEventOwner _owner;

    private void Awake()
    {
        // 從子物件往上找父層的 Enemy
        _owner = GetComponentInParent<IAnimationEventOwner>();
        if (_owner == null)
            Debug.LogError("找不到父層IAnimationEventOwner。");
    }

    public void AnimationEvent_SpawnerSkill() => _owner?.AnimationEvent_SpawnerSkill();

}
