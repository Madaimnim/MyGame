using UnityEngine;
using TMPro;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public GameObject PopupPrefab_Exp;
    public GameObject PopupPrefab_StageClear;


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

    #region 顯示經驗值 
    public void ShowExpPopup(float expValue,Vector3 position) {

        // 生成 Popup
        GameObject popup = Instantiate(PopupPrefab_Exp, position, Quaternion.identity);

        // 直接修改文字 (假設 Prefab 上有 TextMeshPro)
        TMP_Text tmpText = popup.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = $"EXP+{expValue}";
        }
        StartCoroutine(FloatPopup(popup));
    }
    #endregion

    #region 顯示通關
    public void ShowStageClearPopup() {
        PopupPrefab_StageClear.SetActive(true);
        PopupPrefab_StageClear.transform.SetAsLastSibling();

        StartCoroutine(BounceEffect(PopupPrefab_StageClear.transform));
    }
    #endregion

    //字體彈跳
    #region 字體彈跳
    private IEnumerator BounceEffect(Transform popup) {
        float elapsed = 0f;
        float duration = 1f; // 總共 2 秒
        int bounces = 3;
        float height = 200f; // 初始彈跳高度

        Vector3 startPos = popup.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 計算彈跳 (用 sin 波 + 衰減)
            float t = elapsed / duration;
            float yOffset = Mathf.Abs(Mathf.Sin(t * bounces * Mathf.PI)) * height * (1 - t);

            popup.position = startPos + Vector3.up * yOffset;
            yield return null;
        }

        // 確保最後停回原點
        popup.position = startPos;
    }
    #endregion

    //字體向上飄協成
    #region 字體向上飄
    private IEnumerator FloatPopup(GameObject popup) {
        float duration =1f;
        float elasped = 0f;

        Vector3 startPosition = popup.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0,1f,0);

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
