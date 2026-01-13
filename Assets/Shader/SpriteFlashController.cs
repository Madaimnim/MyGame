using UnityEngine;

public class SpriteFlashController {
    private readonly SpriteRenderer _renderer;
    private readonly MaterialPropertyBlock _mpb;

    static readonly int FlashStrengthID = Shader.PropertyToID("_FlashStrength");

    public SpriteFlashController(SpriteRenderer renderer, SpriteEffectPropertyBlock shared) {
        _renderer = renderer;
        _mpb = shared.MPB;
    }

    public void SetFlash(float strength) {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashStrengthID, Mathf.Max(0f, strength));
        _renderer.SetPropertyBlock(_mpb);
    }

    public void Clear() {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashStrengthID, 0f);
        _renderer.SetPropertyBlock(_mpb);
    }
}
