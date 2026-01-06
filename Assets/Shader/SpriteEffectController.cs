using UnityEngine;


public class SpriteEffectController {
    private SpriteRenderer _spriteRenderer;
    private MaterialPropertyBlock _mpb;

    // ===== 狀態快取（重點）=====
    private float _flashStrength = 0f;
    private float _outlineSize = 0f;
    private Color _outlineColor = Color.clear;

    public SpriteEffectController(SpriteRenderer spriteRenderer) {
        _spriteRenderer = spriteRenderer;
        _mpb = new MaterialPropertyBlock();


        Apply(); // 確保初始狀態乾淨
    }

    public void SetFlash(float strength) {
        _flashStrength = Mathf.Max(0, strength);
        Apply();
    }

    public void SetOutline(bool enable, Color color, float size) {
        _outlineSize = enable ? Mathf.Max(0, size) : 0f;
        _outlineColor = color;
        Apply();
    }

    public void ClearAll() {
        _flashStrength = 0f;
        _outlineSize = 0f;
        _outlineColor = Color.clear;

        _spriteRenderer.SetPropertyBlock(null);
    }

    // ===== 核心套用點（唯一寫 MPB 的地方）=====
    private void Apply() {
        _spriteRenderer.GetPropertyBlock(_mpb);

        _mpb.SetFloat("_FlashStrength", _flashStrength);
        _mpb.SetFloat("_OutlineSize", _outlineSize);
        _mpb.SetColor("_OutlineColor", _outlineColor);

        _spriteRenderer.SetPropertyBlock(_mpb);
    }
}
