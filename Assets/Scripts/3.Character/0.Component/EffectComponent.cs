using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class EffectComponent
{
    private Transform _transform;
    private VisualData _visualData;
    private MonoBehaviour _runner;
    private SpriteRenderer _spr;
    private HealthComponent _healthComponent;

    public EffectComponent(VisualData visualData,Transform transform, MonoBehaviour runner,SpriteRenderer spr, HealthComponent healthComponent) {
        _transform = transform;
        _visualData = visualData;
        _runner = runner;
        _spr = spr;
        _healthComponent=healthComponent;
    }

    public void GainedExpEffect(int exp) {
        TextPopupManager.Instance.ShowExpPopup(exp, _transform.position);
    }

    public void PlayerDeathEffect() {
        Color c = _spr.color;
        c.a = 0.5f;  // 透明度 0=完全透明，1=完全不透明，你可以調整成 0.3~0.7
        _spr.color = c;

        VFXManager.Instance.Play("PlayerDeath", _transform.position);
        AudioManager.Instance.PlaySFX(_visualData.DeathSFX, 0.5f);
    }

    public void LevelUpEffect(int newLevel) {
        TextPopupManager.Instance.ShowLevelUpPopup(newLevel, _transform);
    }

    public void TakeDamageEffect(int damage) {
        if(!_healthComponent.IsDead) _runner.StartCoroutine(FlashWhite(1f));
        TextPopupManager.Instance.ShowTakeDamagePopup(damage, _transform); 
    }
    protected virtual IEnumerator FlashWhite(float duration) {
        if (_spr != null)
        {
            _spr.material = _visualData.FlashMaterial;
            yield return new WaitForSeconds(duration);
            _spr.material = _visualData.NormalMaterial;
        }
    }

}
