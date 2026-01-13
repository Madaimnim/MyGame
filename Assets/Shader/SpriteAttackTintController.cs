using UnityEngine;

public class SpriteAttackTintController {
    private readonly SpriteRenderer _renderer;
    private readonly MaterialPropertyBlock _mpb;

    static readonly int AttackColorID = Shader.PropertyToID("_AttackColor");
    static readonly int AttackProgressID = Shader.PropertyToID("_AttackProgress");

    public SpriteAttackTintController(SpriteRenderer renderer, SpriteEffectPropertyBlock shared) {
        _renderer = renderer;
        _mpb = shared.MPB;
    }

    public void SetAttackTint(Color color, float progress) {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(AttackColorID, color);
        _mpb.SetFloat(AttackProgressID, Mathf.Clamp01(progress));
        _renderer.SetPropertyBlock(_mpb);
    }

    public void Clear() {
        SetAttackTint(Color.clear, 0f);
    }
}
