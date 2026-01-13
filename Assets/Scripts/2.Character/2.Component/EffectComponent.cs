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
    private MonoBehaviour _runner;
    private StateComponent _stateComponent;
    private SpriteRenderer _spriteRenderer;

    private readonly SpriteFlashController _spriteFlashController;
    private readonly SpriteOutlineController _spriteOutlineController;
    private readonly SpriteAttackTintController _spriteAttackTintController;
    private readonly SpriteInnerEdgeController _spriteInnerEdgeController;

    private OutlineState _outlineState = OutlineState.None;
    //Test Rarity
    private Rarity _rarity;             //待傳入參數
    private Coroutine _attackTintCoroutine;

    public EffectComponent(Transform transform, MonoBehaviour runner,SpriteRenderer spr,StateComponent stateComponent) {
        _transform = transform;
        _runner = runner;
        _stateComponent = stateComponent;
        _spriteRenderer= spr;

        _spriteOutlineController = new SpriteOutlineController(spr);
        _spriteInnerEdgeController = new SpriteInnerEdgeController(spr);
        
        var sharedMPB = new SpriteEffectPropertyBlock();
        _spriteFlashController =new SpriteFlashController(spr, sharedMPB);
        _spriteAttackTintController =new SpriteAttackTintController(spr, sharedMPB);
    }

    public void GainedExpEffect(int exp) {
        TextPopupManager.Instance.ShowExpPopup(exp, _transform.position);
    }

    public void PlayerDeathEffect() {
        //Color c = _spr.color;
        //c.a = 0.5f;  // 透明度 0=完全透明，1=完全不透明，你可以調整成 0.3~0.7
        //_spr.color = c;

        VFXManager.Instance.Play("PlayerDeath", _transform.position);
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

    //閃白系統
    private IEnumerator FlashWhite(float duration) {
        if (_spriteFlashController == null) yield break;

        _spriteFlashController.SetFlash(0.5f);
        yield return new WaitForSeconds(duration);
        _spriteFlashController.SetFlash(0f);

    }

    //描外框系統
    public void SetOutlineState(OutlineState state) {
        if (_outlineState == state) return;
        _outlineState = state;

        switch (state) {
            case OutlineState.None:
                _spriteOutlineController.SetOutline(false, Color.clear, 0f);
                break;

            case OutlineState.Hover:
                _spriteOutlineController.SetOutline(true, Color.white, 1f);
                break;

            case OutlineState.Target:
                _spriteOutlineController.SetOutline(true, Color.red, 2f);
                break;
        }
    }

    //稀有度系統
    private void ApplyRarityVisual() {
        Color color = RarityColor.Get(_rarity);
        float size = _rarity switch {
            Rarity.Normal => 0f,
            Rarity.Uncommon => 1f,
            Rarity.Rare => 1.2f,
            Rarity.Epic => 1.5f,
            Rarity.Legendary => 2f,
            Rarity.Mythic => 2.5f,
            _ => 0f
        };
        _spriteInnerEdgeController.SetInnerEdge(color, 1f);
    }

    //攻擊變紅協程
    public void PlayAttackTint(Color color, float duration) {
        StopAttackTint(); // 保證不疊加
        _attackTintCoroutine = _runner.StartCoroutine(AttackTintRoutine(color, duration));
    }
    private IEnumerator AttackTintRoutine(Color color, float duration) {
        float t = 0f;

        while (t < duration) {
            // 如果攻擊動畫已經不是播放中 → 中斷
            if (!_stateComponent.IsPlayingAttackAnimation) {
                _spriteAttackTintController.Clear();
                yield break;
            }

            t += Time.deltaTime;
            float progress = Mathf.Clamp01(t / duration);

            _spriteAttackTintController.SetAttackTint(color, progress);
            yield return null;
        }

        // 動畫自然結束
        _spriteAttackTintController.Clear();
    }

    public void StopAttackTint() {
        if (_attackTintCoroutine != null) {
            _runner.StopCoroutine(_attackTintCoroutine);
            _attackTintCoroutine = null;
        }

        // 只清 AttackTint，不碰其他效果
        _spriteAttackTintController.Clear();
    }
}
