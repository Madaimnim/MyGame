using UnityEngine;
using TMPro;
using System.Collections;

public class TextPopupManager : MonoBehaviour
{
    public static TextPopupManager Instance;

    public GameObject TextPrefab_Exp;
    public GameObject TextPrefab_LevelUp;
    public GameObject TextPrefab_StageClear;
    public GameObject TextPrefab_StageDefeat;
    public GameObject TextPrefab_Resume;

    //生命週期
    #region
    private void Awake() {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion


    //顯示升級
    #region 顯示升級
    public void ShowLevelUpPopup(int level, Transform target) {

        GameObject popup = Instantiate(TextPrefab_LevelUp, target);
        popup.transform.localPosition = new Vector3(0, 1.2f, 0); // 偏移到頭頂

        // 直接修改文字 (假設 Prefab 上有 TextMeshPro)
        TMP_Text tmpText = popup.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = $"Level{level}";
        }
        Destroy(popup, 2f);
        //StartCoroutine(FloatPopup(popup));
    }
    #endregion
    //顯示經驗值
    #region 顯示經驗值 
    public void ShowExpPopup(int expValue,Vector3 position) {

        // 生成 Popup
        GameObject popup = Instantiate(TextPrefab_Exp, position, Quaternion.identity);
        popup.transform.localPosition = new Vector3(popup.transform.localPosition.x, popup.transform.localPosition.y+0.7f, popup.transform.localPosition.z); // 偏移到頭頂

        // 直接修改文字 (假設 Prefab 上有 TextMeshPro)
        TMP_Text tmpText = popup.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = $"exp {expValue}";
        }
        StartCoroutine(FloatPopup(popup));
    }
    #endregion

    //顯示通關Text
    #region ShowStageClearPopup()
    public void ShowStageClearPopup() {
        TextPrefab_StageClear.SetActive(true);
        TextPrefab_StageClear.transform.SetAsLastSibling();

        StartCoroutine(BounceEffect(TextPrefab_StageClear.transform));
    }
    #endregion

    //顯示失敗Text
    #region ShowStageDefeatPopup()
    public void ShowStageDefeatPopup() {
        TextPrefab_StageDefeat.SetActive(true);
        TextPrefab_StageDefeat.transform.SetAsLastSibling();

        StartCoroutine(BounceEffect(TextPrefab_StageDefeat.transform));
    }
    #endregion


    //字體彈跳效果
    #region 字體彈跳
    private IEnumerator BounceEffect(Transform textPrefab) {
        float elapsed = 0f;
        float duration = 1f; // 總共 2 秒
        int bounces = 3;
        float height = 200f; // 初始彈跳高度

        Vector3 startPos = textPrefab.position;
             while (elapsed < duration)
             {
                 elapsed += Time.deltaTime;
             
                 // 計算彈跳 (用 sin 波 + 衰減)
                 float t = elapsed / duration;
                 float yOffset = Mathf.Abs(Mathf.Sin(t * bounces * Mathf.PI)) * height * (1 - t);
             
             textPrefab.position = startPos + Vector3.up * yOffset;
             yield return null;
            }



    }
    #endregion

    //字體向上飄效果
    #region 字體向上飄
    private IEnumerator FloatPopup(GameObject popup) {
        float duration =0.8f;
        float elasped = 0f;

        Vector3 startPosition = popup.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0,0.7f,0);

        while (elasped < duration)
        {
            elasped += Time.deltaTime;
            float t = elasped / duration;
            if (popup != null)
            {
                popup.transform.position = Vector3.Lerp(startPosition, endPosition,t);
                yield return null;
            }
        }


        if (popup != null)
            Destroy(popup);
    }
    #endregion
}
