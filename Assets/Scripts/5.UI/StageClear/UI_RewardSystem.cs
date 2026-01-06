using System.Collections;
using TMPro;
using UnityEngine;

public class UI_RewardSystem : MonoBehaviour {
    public RectTransform ImageRrect;

    [Header("結算文字顯示")]
    public TMP_Text InfoText;
    public string PrefixText = "經驗值：";

    [Header("速度 (pixel / sec)")]
    public float ResizeSpeed = 4000f;

    [Header("尺寸")]
    public float MinWidth = 0f;
    public float MaxWidth = 1500f;
    public float MinHeight = 20f;
    public float MaxHeight = 700f;

    public System.Action<float> OnExpandHeightChanged;

    Coroutine _resizeCoroutine;

    private void Awake() {
        HideText();
    }

    private void OnEnable() {
        ImageRrect.sizeDelta = new Vector2(MinWidth, MinHeight);
        HideText();

        if (_resizeCoroutine != null) StopCoroutine(_resizeCoroutine);

        _resizeCoroutine = StartCoroutine(ExpandWidthThenHeight());
    }

    private void OnDisable() {
        if (_resizeCoroutine != null) {
            StopCoroutine(_resizeCoroutine);
            _resizeCoroutine = null;
        }
    }

    IEnumerator ExpandWidthThenHeight() {
        // Phase 1：開寬
        float widthSpeedFactor = (MaxHeight > 0f) ? MaxWidth / MaxHeight : 1f;

        while (!Mathf.Approximately(ImageRrect.sizeDelta.x, MaxWidth)) {
            float w = Mathf.MoveTowards(ImageRrect.sizeDelta.x, MaxWidth, ResizeSpeed * Time.deltaTime * widthSpeedFactor);

            ImageRrect.sizeDelta = new Vector2(w, ImageRrect.sizeDelta.y);
            yield return null;
        }

        // Phase 2：開高
        while (!Mathf.Approximately(ImageRrect.sizeDelta.y, MaxHeight)) {
            float h = Mathf.MoveTowards(ImageRrect.sizeDelta.y, MaxHeight, ResizeSpeed * Time.deltaTime);


            ImageRrect.sizeDelta = new Vector2(ImageRrect.sizeDelta.x, h);

            float currentHeightOffset = ImageRrect.sizeDelta.y - MinHeight;
            OnExpandHeightChanged?.Invoke(currentHeightOffset);

            yield return null;
        }

        OnExpandFinished();
    }

    void OnExpandFinished() {
        ShowExperience();
    }
    void ShowExperience() {
        if (ExperienceManager.Instance == null) {
            Debug.LogWarning("ExperienceManager not found.");
            return;
        }

        int exp = ExperienceManager.Instance.GetCurrentExp();
        ShowText(exp);
    }

    public void ShowText(int value) {
        if (InfoText == null) return;

        InfoText.text = PrefixText + value;
        InfoText.gameObject.SetActive(true);
    }
    public void HideText() {
        if (InfoText == null) return;

        InfoText.gameObject.SetActive(false);
    }
    public void SetSize(float width, float height) {
        ImageRrect.sizeDelta = new Vector2(width, height);
    }

}
