using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class ShadowController : MonoBehaviour {
    [Header("Reference")]
    private Transform _spriteTransform;
    private SpriteRenderer _shadowRenderer;

    [Header("Height Mapping")]


    private ShadowConfig _shadowConfig;

    private void Awake() {

        _shadowRenderer = GetComponent<SpriteRenderer>();
        _shadowConfig = GameSettingManager.Instance.ShadowConfig;

        // 找父階層中「不是自己」的 SpriteRenderer
        var IInteractable = GetComponentInParent<IInteractable>();
        if (IInteractable == null) Debug.LogError("ShadowController: 找不到 IInteractable");
        else _spriteTransform = IInteractable.SpriteTransform;
    }

    private void LateUpdate() {
        if (_spriteTransform == null) return;

        float spriteY = _spriteTransform.localPosition.y;

        UpdateRotation();
        UpdatePosition(spriteY);
        UpdateScale(spriteY);
        UpdateAlpha(spriteY);
    }

    // ===== 同步方法 =====

    private void UpdateRotation() {
        transform.rotation = _spriteTransform.rotation;
    }



    private void UpdatePosition(float spriteY) {
        float shadowY = -Mathf.Max(0f, spriteY * _shadowConfig.ShadowHeightRatio);

        Vector3 pos = transform.localPosition;
        pos.y = shadowY;
        transform.localPosition = pos;
    }

    private void UpdateScale(float spriteY) {
        float t = Mathf.Clamp01(spriteY / _shadowConfig.FadeOutHeight);

        float heightStretch = Mathf.Lerp(1f,_shadowConfig.MaxHeightStretch,t);
        float sunStretch = _shadowConfig.SunLengthMultiplier;

        Vector3 spriteScale = _spriteTransform.localScale;

        transform.localScale = new Vector3(
            spriteScale.x ,
            -spriteScale.y * heightStretch * sunStretch,
            spriteScale.z
        );




        //var multi = _shadowConfig.LengthMultiplier;
        //Vector3 newScale = new Vector3(
        //    _spriteTransform.localScale.x* multi,
        //    _spriteTransform.localScale.y * multi,
        //    _spriteTransform.localScale.z);
        //
        //transform.localScale = newScale;

    }

    private void UpdateAlpha(float spriteY) {
        float t = Mathf.Clamp01(spriteY / _shadowConfig.FadeOutHeight);
        float alpha = Mathf.Lerp(_shadowConfig.MaxAlpha, _shadowConfig.MinAlpha, t);

        Color c = _shadowRenderer.color;
        c.a = alpha;
        _shadowRenderer.color = c;
    }
}
