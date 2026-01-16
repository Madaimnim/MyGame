using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjectAnimationEvent : MonoBehaviour
{
    private SkillObject _skillObject;
    private void Awake() {
        _skillObject = GetComponentInParent<SkillObject>();
    }
    public void AnimationEvent_Destroy() {
        Destroy(_skillObject.gameObject);
    }

}
