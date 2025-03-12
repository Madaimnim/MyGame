using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameWallSliderController : MonoBehaviour
{
    #region 公開變數
    [Header("拖曳設置Enemy、Slider物件")]
    public Slider slider; // Slider
    private GameWall gameWall;

    [Header("拖曳設置TextMeshPro物件")]
    public TMP_Text text; // 數值顯示
    [Header("血條顯示標題")]
    public string title = "Null"; // 血條顯示的標題（可設定）
    #endregion

    #region 私有變數

    #endregion

    private void Awake() {
        gameWall = GetComponent<GameWall>();
        if (gameWall != null)
        {
            gameWall.ValueChanged += UpdateUIValue;            //監聽Enemy的血量變化
        }
        else
            Debug.LogError("gameWall為空!");
    }


    #region Start()方法
    private void Start() {
    }

    #endregion

    #region  LateUpdate()方法，於Update() 之後才更新
    private void LateUpdate() {

    }
    #endregion

    #region 私有UpdateUIValue(int currentValue, int maxValue)方法
    private void UpdateUIValue(int currentValue, int maxValue) {    //監聽Enemy上的ValueChanged事件，更新UISlider上的數值
        if (slider != null)
        {
            slider.maxValue = maxValue; // ✅ 設定最大值
            slider.value = currentValue; // ✅ 更新當前值
        }

        if (text != null)
        {
            text.text = $"{title}: {currentValue} / {maxValue}"; // ✅ 更新顯示文字
        }
    }
    #endregion

}

