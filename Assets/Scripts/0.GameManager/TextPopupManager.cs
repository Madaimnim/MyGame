using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TextPopupManager : MonoBehaviour
{
    public static TextPopupManager Instance{ get; private set; }

    public GameObject TextPrefab_Exp;
    public GameObject TextPrefab_LevelUp;
    public GameObject TextPrefab_StageClear;
    public GameObject TextPrefab_StageDefeat;
    public GameObject TextPrefab_Resume;
    public GameObject TextPrefab_Damage;
    public GameObject TextPrafab_RespawnTimer;
    [Header("受傷害字體")]
    public GameObject TextPrefab_TakeDamage;


    private Dictionary<Transform, List<GameObject>> activeLevelUpPopups = new Dictionary<Transform, List<GameObject>>();
    private Dictionary<Transform, List<GameObject>> activeTakeDamagePopups = new Dictionary<Transform, List<GameObject>>();

    //生命週期
    #region 生命週期
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    private void OnEnable() {
        if(GameEventSystem.Instance!=null) GameEventSystem.Instance.Event_SkillLevelUp += ShowSkillLevelUpPopup;
    }
    private void OnDisable() {
        if (GameEventSystem.Instance != null) GameEventSystem.Instance.Event_SkillLevelUp -= ShowSkillLevelUpPopup;
    }

    //顯示玩家復活倒數
    #region ShowRespawnTimerPopup(Transform target,float respanwTime)
    public void ShowRespawnTimerPopup(Transform target,float respanwTime) {
        Vector3 basePos = target.position + new Vector3(0,1f,0);
        GameObject newPopup = Instantiate(TextPrafab_RespawnTimer,basePos,Quaternion.identity);
        TMP_Text tmpText = newPopup.GetComponentInChildren<TMP_Text>();
        if (tmpText == null) return;

        StartCoroutine(UpdateRespawnTimer(tmpText, respanwTime));
    }

    private IEnumerator UpdateRespawnTimer(TMP_Text tmpText ,float time) {
        for (float t=time;t>0;t--)
        {
            tmpText.text =$"{t}";
            yield return new WaitForSeconds(1);
        }
        Destroy(tmpText);

    }
    #endregion

    //顯示受傷害字樣
    #region ShowTakeDamagePopup(int damage, Transform target)
    public void ShowTakeDamagePopup(int damage, Transform target) {
        if (target == null) return;

        // 基準位置（頭頂）
        Vector3 basePos = target.position + new Vector3(0, 1.2f, 0);

        // 在一個小範圍內隨機偏移
        float range = 0.5f; // 隨機範圍大小
        Vector3 randomOffset = new Vector3(
            Random.Range(-range, range),
            Random.Range(-range * 0.8f, range * 0.3f), // Y 軸也可以小幅隨機
            0
        );

        Vector3 spawnPos = basePos + randomOffset;

        // 生成 Popup (不要掛在 target 底下，而是世界座標)
        GameObject newPopup = Instantiate(TextPrefab_TakeDamage, spawnPos, Quaternion.identity);

        // 文字設定
        TMP_Text tmpText = newPopup.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = $"{damage}";
        }

        // 啟動浮動效果協程
        StartCoroutine(FloatDamagePopup(newPopup));
    }
    #endregion

    //顯示造成傷害字樣
    #region ShoDamagePopup(int damage, Transform target)
    public void ShowDamagePopup(int damage, Transform target) {
        if (target == null) return;

        // 基準位置（頭頂）
        Vector3 basePos = target.position + new Vector3(0, 1.2f, 0);

        // 在一個小範圍內隨機偏移
        float range = 0.5f; // 隨機範圍大小
        Vector3 randomOffset = new Vector3(
            Random.Range(-range, range),
            Random.Range(-range * 0.8f, range * 0.3f), // Y 軸也可以小幅隨機
            0
        );

        Vector3 spawnPos = basePos + randomOffset;

        // 生成 Popup (不要掛在 target 底下，而是世界座標)
        GameObject newPopup = Instantiate(TextPrefab_Damage, spawnPos, Quaternion.identity);

        // 文字設定
        TMP_Text tmpText = newPopup.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = $"{damage}";
        }

        // 啟動浮動效果協程
        StartCoroutine(FloatDamagePopup(newPopup));
    }

    #endregion

    //顯示人物升級
    #region  ShowLevelUpPopup(int level, Transform target)
    public void ShowLevelUpPopup(int level, Transform target) {
        CreateLevelUpPopup(TextPrefab_LevelUp, $"Level {level}", target);
    }
    #endregion

    //顯示技能升級
    #region ShowSkillLevelUpPopup(string skillName, int level, Transform target)
    public void ShowSkillLevelUpPopup(PlayerSkillRuntime playerSkillDataRuntime, Transform ownerTransform) {
        CreateLevelUpPopup(TextPrefab_LevelUp, $"{playerSkillDataRuntime.StatsData.Name}Lv.{playerSkillDataRuntime.SkillLevel}", ownerTransform);
    }
    #endregion

    //共用方法：建立 Popup入駐列
    #region CreateLevelUpPopup(GameObject prefab, string text, Transform target)
    private void CreateLevelUpPopup(GameObject prefab, string text, Transform target) {
        if (target == null) return;

        if (!activeLevelUpPopups.ContainsKey(target))
        {
            activeLevelUpPopups[target] = new List<GameObject>();
        }

        float baseY = 1.2f;
        float offsetStep = 0.4f;

        // 先把舊的 Popup 全部往上推
        foreach (var popup in activeLevelUpPopups[target])
        {
            if (popup != null)
            {
                popup.transform.localPosition += new Vector3(0, offsetStep, 0);
            }
        }

        // 新 Popup 固定在基準位置
        GameObject newPopup = Instantiate(prefab, target);
        newPopup.transform.localPosition = new Vector3(0, baseY, 0);

        TMP_Text tmpText = newPopup.GetComponentInChildren<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = text;
        }

        activeLevelUpPopups[target].Add(newPopup);
        StartCoroutine(RemovePopupAfterDelay(newPopup, target, 2f));
    }

    private IEnumerator RemovePopupAfterDelay(GameObject popup, Transform target, float delay) {
        yield return new WaitForSeconds(delay);

        if (popup != null) Destroy(popup);
        if (target != null && activeLevelUpPopups.ContainsKey(target))
        {
            activeLevelUpPopups[target].Remove(popup);
        }
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

    //傷害、受傷害字體隨機生成、縮放、漂浮效果
    #region FloatDamagePopup(GameObject popup)
    private IEnumerator FloatDamagePopup(GameObject popup) {
        float duration = 0.7f;
        float elapsed = 0f;

        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1.2f, 0); // 向上移動 1 單位

        while (elapsed < duration)
        {
            if (popup == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 線性往上 + 一點縮放效果
            popup.transform.position = Vector3.Lerp(startPos, endPos, t);

            float scale = 1 + Mathf.Sin(t * Mathf.PI) * 0.8f; // 微彈縮放
            popup.transform.localScale = Vector3.one * scale;

            yield return null;
        }

        if (popup != null) Destroy(popup);
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
