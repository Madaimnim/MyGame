using System;
using UnityEngine;

public class CharEffectComponent
{
    private Transform _transform;
    private VisualData _visualData;
    
    public CharEffectComponent(VisualData visualData,Transform transform) {
        _transform = transform;
        _visualData = visualData;
    }

    public void PlayDeathEffect() {
        VFXManager.Instance.Play("PlayerDeath", _transform.position);
        AudioManager.Instance.PlaySFX(_visualData.DeathSFX, 0.5f);
    }
}
