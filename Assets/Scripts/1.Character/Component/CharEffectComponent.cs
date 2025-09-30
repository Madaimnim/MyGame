using System;
using UnityEngine;

public class CharEffectComponent
{
    private Transform _transform;
    private PlayerStatsRuntime _rt;
    
    public CharEffectComponent(PlayerStatsRuntime rt,Transform transform) {
        _transform = transform;
        _rt = rt;
    }

    public void PlayDeathEffect() {
        VFXManager.Instance.Play("PlayerDeath", _transform.position);
        AudioManager.Instance.PlaySFX(_rt.VisualData.DeathSFX, 0.5f);
    }
}
