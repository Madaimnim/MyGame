using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Player _player;

    private void Awake() {
        _player = GetComponentInParent<Player>();
        if (_player == null) Debug.LogWarning("Player is Null!");
    }

    public void AnimationEvent_SpawnerSkill() {
        _player.SkillComponent.UseSkill();
    }

    public void AnimationEvent_SkillDashStart(int skillId) {
        if (!_player.Rt.SkillPool.TryGetValue(skillId, out var skillRt)) return;

        _player.SkillComponent.UseSkill();

        _player.SkillComponent.SkillDashMove(skillRt);
    }
}
