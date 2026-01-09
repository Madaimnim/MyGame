using UnityEngine;

public class SpriteOutlineController {
    private readonly SpriteRenderer _spriteRenderer;
    private readonly MaterialPropertyBlock _mpb;

    private float _outlineSize = 0f;
    private Color _outlineColor = Color.clear;

    public SpriteOutlineController(SpriteRenderer spriteRenderer) {
        _spriteRenderer = spriteRenderer;
        _mpb = new MaterialPropertyBlock();
        Apply();
    }

    public void SetOutline(bool enable, Color color, float size) {
        _outlineSize = enable ? Mathf.Max(0f, size) : 0f;
        _outlineColor = color;
        Apply();
    }

    public void Clear() {
        _outlineSize = 0f;
        _outlineColor = Color.clear;
        Apply();
    }

    private void Apply() {
        _spriteRenderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_OutlineSize", _outlineSize);
        _mpb.SetColor("_OutlineColor", _outlineColor);
        _spriteRenderer.SetPropertyBlock(_mpb);
    }
}
