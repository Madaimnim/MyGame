using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SkillCastMode {
    Instant,
    HoldRelease
}

[System.Serializable]
[CreateAssetMenu(fileName = "InputSettings",menuName = "GameSettings/InputSettings")]
public class InputSettings : ScriptableObject
{

        [Header("Skill Casting")]
        public SkillCastMode SkillCastMode = SkillCastMode.Instant;

        //public bool ShowDetectorOnHold = true;
        //public bool AllowCancelSkill = false;
}
