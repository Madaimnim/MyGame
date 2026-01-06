using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;
public enum OutlineState {
    None,
    Hover,
    Target
}
public class EffectComponent
{
    private Transform _transform;
    private VisualData _visualData;
    private MonoBehaviour _runner;
    private StateComponent _stateComponent;
    private SpriteRenderer _spriteRenderer;

    private SpriteEffectController _spriteEffectController;
    private OutlineState _outlineState = OutlineState.None;

    public EffectComponent(VisualData visualData,Transform transform, MonoBehaviour runner,SpriteRenderer spr,StateComponent stateComponent) {
        _transform = transform;
        _visualData = visualData;
        _runner = runner;
        _stateComponent = stateComponent;
        _spriteRenderer= spr;

        _spriteEffectController = new SpriteEffectController(_spriteRenderer);
    }

    public void GainedExpEffect(int exp) {
        TextPopupManager.Instance.ShowExpPopup(exp, _transform.position);
    }

    public void PlayerDeathEffect() {
        //Color c = _spr.color;
        //c.a = 0.5f;  // 透明度 0=完全透明，1=完全不透明，你可以調整成 0.3~0.7
        //_spr.color = c;

        VFXManager.Instance.Play("PlayerDeath", _transform.position);
        AudioManager.Instance.PlaySFX(_visualData.DeathSFX, 0.5f);
    }

    public void LevelUpEffect(int newLevel) {
        TextPopupManager.Instance.ShowLevelUpPopup(newLevel, _transform);
    }

    public void TakeDamageEffect(int damage) {
        if(!_stateComponent.IsDead) _runner.StartCoroutine(FlashWhite(0.1f));
        TextPopupManager.Instance.ShowTakeDamagePopup(damage, _transform); 
    }

    public void ShowHoverOutline() => SetOutlineState(OutlineState.Hover);
    public void ShowTargetOutline() => SetOutlineState(OutlineState.Target);
    public void HideOutline() => SetOutlineState(OutlineState.None);


    private IEnumerator FlashWhite(float duration) {
        if (_spriteEffectController == null) yield break;

        _spriteEffectController.SetFlash(0.8f);
        yield return new WaitForSeconds(duration);
        _spriteEffectController.SetFlash(0f);

    }

    public void SetOutlineState(OutlineState state) {
        if (_outlineState == state) return;
        _outlineState = state;

        switch (state) {
            case OutlineState.None:
                _spriteEffectController.SetOutline(false, Color.clear, 0f);
                break;

            case OutlineState.Hover:
                _spriteEffectController.SetOutline(true, Color.white, 1f);
                break;

            case OutlineState.Target:
                _spriteEffectController.SetOutline(true, Color.red, 2f);
                break;
        }
    }
}
