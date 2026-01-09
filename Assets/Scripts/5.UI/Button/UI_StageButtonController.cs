using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class UI_StageButtonController : MonoBehaviour
{
    public RectTransform MainPanelTransform;  
    public RectTransform SubPanelTransform;   
    [Header("主清單向上位移高度")]
    public int FloatUpPixels = 1000;
    public float FloutDuration = 0.25f;

    private Vector3 _mainPanelDefaultPos;      // 記錄主 CanvasPanel 的初始位置

    //生命週期
    private void Awake() {
        if (MainPanelTransform != null)
            _mainPanelDefaultPos = MainPanelTransform.anchoredPosition;
    }

    private void OnEnable() {
        // 主 Canvas 回到原始位置並啟用
        if (MainPanelTransform != null)
        {
            MainPanelTransform.gameObject.SetActive(true);
            MainPanelTransform.anchoredPosition = _mainPanelDefaultPos;
        }

        // 關閉所有子 Canvas
        SubPanelTransform.gameObject.SetActive(false);
    }

    // Inspector 綁定：章節按鈕
    public void OnMainStageClicked(int stageMainNumber) {        
        if (SubPanelTransform==null) return;
        SubPanelTransform.anchoredPosition = new Vector3(SubPanelTransform.anchoredPosition.x, SubPanelTransform.anchoredPosition.y - FloatUpPixels);
        SubPanelTransform.gameObject.SetActive(true);

        StartCoroutine(SmoothMove(FloutDuration));
    }

    // Inspector 綁定：子關卡按鈕
    public void OnSubStageClicked(int stageId) {
        if(GameManager.Instance.GameStageSystem == null) return;
        GameManager.Instance.GameStageSystem.RequestEnterStage(stageId);
    }

    private IEnumerator SmoothMove(float duration) {
        float t = 0f;
        var startMainPos = MainPanelTransform.anchoredPosition;
        var startSubPos = SubPanelTransform.anchoredPosition;
        var movVectro = new Vector2(0, FloatUpPixels);

        while (t < duration) {
            t += Time.deltaTime;
            MainPanelTransform.anchoredPosition = Vector3.Lerp(startMainPos, startMainPos + movVectro, t / duration);
            SubPanelTransform.anchoredPosition = Vector3.Lerp(startSubPos, startSubPos + movVectro, t / duration);
            yield return null;
        }
        MainPanelTransform.gameObject.SetActive(false);
    }
}
