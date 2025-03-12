using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ShadowController : MonoBehaviour
{
    [Header("影子透明度")]
    [Range(0f, 1f)]
    public float baseOpacity = 0.5f;
    [Range(0f, 1f)]
    public float minOpacity = 0.1f;
    [Range(0f, 1f)]
    public float maxOpacity = 0.8f;

    [Header("间隔")]
    public float shrinkDuration = 0.1f;
    public float holdDuration = 0f;
    public float restoreDuration = 0.1f;

    [Header("缩放设置")]
    public float minScale = 0.5f;

    [Header("位移设置")]
    public Vector2 offsetAmount = new Vector2(0.1f, 0.1f);

    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private Color originalColor;
    private Vector3 originalPosition;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("❌ 未找到 SpriteRenderer 组件！");
            enabled = false;
            return;
        }

        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        originalColor = spriteRenderer.color;
        originalColor.a = baseOpacity;
        spriteRenderer.color = originalColor;
    }

    public void AdjustShadowAlpha() {// 调用此方法以控制影子的透明度、缩放和位移
        StopAllCoroutines();
        StartCoroutine(AdjustShadowCoroutine());
    }

    private IEnumerator AdjustShadowCoroutine() {
        float timer = 0f;

        // ✅ 影子变淡 & 缩小 & 位移
        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / shrinkDuration);

            // 透明度变化
            Color color = originalColor;
            color.a = Mathf.Lerp(baseOpacity, minOpacity, progress);
            spriteRenderer.color = color;

            // 缩放变化
            float scaleValue = Mathf.Lerp(originalScale.x, minScale, progress);
            transform.localScale = new Vector3(scaleValue, scaleValue, originalScale.z);

            // 位移变化
            transform.localPosition = originalPosition + Vector3.Lerp(Vector3.zero, offsetAmount, progress);

            yield return null;
        }

        // ✅ 最淡状态保持
        yield return new WaitForSeconds(holdDuration);

        timer = 0f;

        // ✅ 影子恢复原状
        while (timer < restoreDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / restoreDuration);

            // 透明度变化
            Color color = originalColor;
            color.a = Mathf.Lerp(minOpacity, baseOpacity, progress);
            spriteRenderer.color = color;

            // 缩放变化
            float scaleValue = Mathf.Lerp(minScale, originalScale.x, progress);
            transform.localScale = new Vector3(scaleValue, scaleValue, originalScale.z);

            // 位移变化
            transform.localPosition = originalPosition + Vector3.Lerp(offsetAmount, Vector3.zero, progress);

            yield return null;
        }

        // ✅ 最后确保完全恢复到初始状态
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
        spriteRenderer.color = originalColor;
    }
}
