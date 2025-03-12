using UnityEngine;
using TMPro;

public class DamageTextController : MonoBehaviour
{
    public float moveSpeed = 1.5f;  // 文字漂浮速度
    public float fadeDuration = 1f; // 淡出時間
    private TMP_Text damageText;
    private Color textColor;

    public void Setup(int damage) {
        damageText = GetComponentInChildren<TMP_Text>();
        textColor = damageText.color;

        damageText.text = $"-{damage}"; //  設定傷害數字
        StartCoroutine(FadeOutAndMove()); //  啟動動畫
    }

    private System.Collections.IEnumerator FadeOutAndMove() {
        float timer = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * 1f; // ✅ 讓文字漂浮一點點

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, timer / fadeDuration);
            textColor.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            damageText.color = textColor;
            yield return null;
        }
        Destroy(gameObject); // ✅ 動畫完成後銷毀
    }
}
