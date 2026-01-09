using UnityEngine;

public class SpriteInnerEdgeController {
    private readonly SpriteRenderer _renderer;
    private readonly MaterialPropertyBlock _mpb;

    private static readonly int InnerEdgeColorID = Shader.PropertyToID("_InnerEdgeColor");
    private static readonly int InnerEdgeSizeID = Shader.PropertyToID("_InnerEdgeSize");

    public SpriteInnerEdgeController(SpriteRenderer renderer) {
        _renderer = renderer;
        _mpb = new MaterialPropertyBlock();
    }

    public void SetInnerEdge(Color color, float size) {
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetColor(InnerEdgeColorID, color);
        _mpb.SetFloat(InnerEdgeSizeID, size);

        _renderer.SetPropertyBlock(_mpb);
    }

    public void ClearInnerEdge() {
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetFloat(InnerEdgeSizeID, 0f);
        _renderer.SetPropertyBlock(_mpb);
    }
}
