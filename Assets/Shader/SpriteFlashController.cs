using UnityEngine;

public class SpriteFlashController {
    private readonly SpriteRenderer _spriteRenderer;
    private readonly MaterialPropertyBlock _mpb;

    public SpriteFlashController(SpriteRenderer spriteRenderer) {
        _spriteRenderer = spriteRenderer;
        _mpb = new MaterialPropertyBlock();
    }

    public void SetFlash(float strength) {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_FlashStrength", Mathf.Max(0f, strength));
        _spriteRenderer.SetPropertyBlock(_mpb);
    }

    public void Clear() {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_FlashStrength", 0f);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }
}
