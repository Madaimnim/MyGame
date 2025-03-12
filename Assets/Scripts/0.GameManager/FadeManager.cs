using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    #region 公開定義
    public static FadeManager Instance;
    [Header("#場景切換用：黑色UI_Image-------------------------------------------------------------------")]
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    #endregion

    #region Awake()方法
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持跨場景存在
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Start()方法
    private void Start() {
    }
    #endregion

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
}
