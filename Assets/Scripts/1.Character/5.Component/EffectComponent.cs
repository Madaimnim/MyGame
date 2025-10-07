using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;

public class EffectComponent
{
    private Transform _transform;
    private VisualData _visualData;
    private ICoroutineRunner _runner;
    private SpriteRenderer _spr;
    
    public EffectComponent(VisualData visualData,Transform transform,ICoroutineRunner runner,SpriteRenderer spr) {
        _transform = transform;
        _visualData = visualData;
        _runner = runner;
        _spr = spr;
    }

    public void GainedExpEffect(int exp) {
        TextPopupManager.Instance.ShowExpPopup(exp, _transform.position);
    }

    public void PlayerDeathEffect() {
        Color c = _spr.color;
        c.a = 0.5f;  // �z���� 0=�����z���A1=�������z���A�A�i�H�վ㦨 0.3~0.7
        _spr.color = c;

        VFXManager.Instance.Play("PlayerDeath", _transform.position);
        AudioManager.Instance.PlaySFX(_visualData.DeathSFX, 0.5f);
    }

    public void LevelUpEffect(int newLevel) {
        TextPopupManager.Instance.ShowLevelUpPopup(newLevel, _transform);
    }

    public void TakeDamageEffect(DamageInfo info) {
        _runner.StartCoroutine(FlashWhite(1f));
        TextPopupManager.Instance.ShowTakeDamagePopup(info.Damage, _transform); 
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
