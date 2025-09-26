using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StageSelectUI : MonoBehaviour
{
    public GameObject mainStageCanvas;      // 第一層 Canvas
    public GameObject[] subStageCanvases;   // 每個章節對應的子 Canvas

    private GameObject currentSubStageCanvas; // 記錄當前開啟的子 Canvas
    private Vector3 mainPanelDefaultPos;      // 記錄主 CanvasPanel 的初始位置

    //生命週期
    #region 生命週期
    private void Awake() {
        if (mainStageCanvas != null)
            mainPanelDefaultPos = mainStageCanvas.GetComponent<RectTransform>().GetChild(0).localPosition;
    }

    private void OnEnable() {
        // 主 Canvas 回到原始位置並啟用
        if (mainStageCanvas != null)
        {
            mainStageCanvas.SetActive(true);
            mainStageCanvas.GetComponent<RectTransform>().GetChild(0).localPosition = mainPanelDefaultPos;
        }

        // 關閉所有子 Canvas
        foreach (var canvas in subStageCanvases)
        {
            if (canvas != null)
                canvas.SetActive(false);
        }

        currentSubStageCanvas = null;
    }
    #endregion

    // Inspector 綁定：點章節按鈕，傳入關卡stageMainNumber
    #region OnMainStageClicked(int stageMainNumber)
    public void OnMainStageClicked(int stageMainNumber) {
        if (stageMainNumber < 1 || stageMainNumber > subStageCanvases.Length)
            return;

        // 主 Canvas 向上滑出去並關閉
        RectTransform mainPanelTransform = mainStageCanvas.transform.GetChild(0).GetComponent<RectTransform>();
        Vector3 mainStart = mainPanelTransform.localPosition;
        Vector3 mainEnd = mainStart + new Vector3(0, 1000, 0);

        StartCoroutine(SmoothMove(mainPanelTransform, mainStart, mainEnd, 0.25f, () => {
            mainStageCanvas.SetActive(false);
        }));

        // 開啟對應的子 Canvas
        currentSubStageCanvas = subStageCanvases[stageMainNumber - 1];
        if (currentSubStageCanvas != null)
        {
            currentSubStageCanvas.SetActive(true);

            // 找 Canvas 內的 Panel (建議子 Canvas 下放一個 Panel 當容器)
            RectTransform subPanelTransform = currentSubStageCanvas.transform.GetChild(0).GetComponent<RectTransform>();
            if (subPanelTransform != null)
            {
                // 把起始位置設在畫面左邊外
                Vector3 targetPos = subPanelTransform.localPosition;
                subPanelTransform.localPosition = new Vector3(targetPos.x, targetPos.y-1000, targetPos.z);

                // 從外部移動到中間
                StartCoroutine(SmoothMove(subPanelTransform, subPanelTransform.localPosition, targetPos, 0.25f));
            }
        }
    }
    #region SmoothMove
    private IEnumerator SmoothMove(RectTransform rect, Vector3 startPos, Vector3 targetPos, float duration, System.Action onComplete = null) {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            rect.localPosition = Vector3.Lerp(startPos, targetPos, t / duration);
            yield return null;
        }

        rect.localPosition = targetPos;
        onComplete?.Invoke();
    }
    #endregion

    #endregion

    // Inspector綁定：回上一層
    #region OnBackToMainStage()
    public void OnBackToMainStage() {
        // 關閉目前子 Canvas
        if (currentSubStageCanvas != null)
        {
            currentSubStageCanvas.SetActive(false);
            currentSubStageCanvas = null;
        }

        // 顯示主選單
        mainStageCanvas.SetActive(true);
    }
    #endregion

    // Inspector 綁定：點子關卡
    #region OnSubStageClicked(string sceneName)
    public void OnSubStageClicked(string sceneName) {
        GameManager.Instance.GameStateSystem.SetState(GameStateSystem.GameState.Battle, sceneName);
    }
    #endregion

}
