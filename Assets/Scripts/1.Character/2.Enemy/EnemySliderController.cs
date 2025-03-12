using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemySliderController : MonoBehaviour
{
    #region 公開變數
    public Enemy enemy;
    public Slider slider; 
    public TMP_Text text; 
    public string TextTitle = "Null"; // 血條顯示的標題（可設定）
    public bool isFollowTarget = false; // 是否讓血條跟隨目標移動
    public Vector2 PositionOffset = new Vector2(0, 50f); // 血條相對於目標的偏移量
    #endregion
    #region 私有變數
    private Camera mainCamera;
    private RectTransform sliderRectTransform;
    #endregion
    #region OnEnable()方法
    private void OnEnable() {
        enemy.Event_HpChanged += UpdateUIValue;
    }
    #endregion
    #region ODisable()方法
    private void OnDisable() {
        enemy.Event_HpChanged -= UpdateUIValue;
    }
    #endregion
    #region Start()方法
    private void Start() {
        mainCamera = Camera.main; // 取得主攝影機
    }
    #endregion

    #region  LateUpdate()方法，於Update() 之後才更新
    private void LateUpdate() {
        if (isFollowTarget && enemy != null)
        {
            UpdatePosition(); // 更新血條 UI 位置
        }
    }
    #endregion

    #region 私有UpdateUIValue(int currentValue, int maxValue)方法
    private void UpdateUIValue(int currentValue, int maxValue) {    //監聽Enemy上的ValueChanged事件，更新UISlider上的數值
        if (slider != null)
        {
            slider.maxValue = maxValue; 
            slider.value = currentValue;
        }
        if (text != null)
        {
            text.text = $"{TextTitle}: {currentValue} / {maxValue}"; // ✅ 更新顯示文字
        }
    }
    #endregion
    #region 私有UpdatePosition()方法，更新UI位置跟隨target目標
    private void UpdatePosition() {         //更新血條 UI 位置（根據目標位置 + 偏移量）
        if (mainCamera != null && enemy != null && sliderRectTransform != null)
        {
            Vector3 worldPosition = enemy.transform.position; // ✅ 取得敵人的世界座標
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition); // ✅ 轉換為螢幕座標

            // ✅ 修正 UI 位置（加上 UI 偏移）
            sliderRectTransform.position = screenPosition + (Vector3)PositionOffset;
        }
    }
    #endregion
}
