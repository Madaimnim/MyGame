using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    #region 公開定義
    public static FadeManager Instance { get; private set; }
    [Header("#場景切換用：黑色UI_Image-------------------------------------------------------------------")]
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    #endregion

    #region Awake()方法
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Start()方法
    private void Start() {
    }
    #endregion

    //變全黑
    #region 公開協程FadeOut()
    public IEnumerator FadeOut() 
    {
        Color color = fadeImage.color;
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration);//淡出
            fadeImage.color = color;
            yield return null;
        }
    }
    #endregion
    //變全透明
    #region  公開協程FadeIn()
    public IEnumerator FadeIn() {
        Color color = fadeImage.color;
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1, 0, timer / fadeDuration);//淡入
            fadeImage.color = color;
            yield return null;
        }
    }
    #endregion
    //變透明(呼叫給予透明度)
    #region 公開方法：直接設置透明度
    public void FadeSetAlpha(float targetAlpha) {
        if (fadeImage == null) return;

        Color color = fadeImage.color;
        color.a = targetAlpha;
        fadeImage.color = color;
    }
    #endregion
}
