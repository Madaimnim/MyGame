using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_HpSlider : MonoBehaviour
{
    #region 公開變數

    [Header("Slider物件")]
    public Slider slider;
    [Header("TextMeshPro數值顯示")]
    public TMP_Text text;
    [Header("血條顯示標題")]
    public string title = "HP"; 
    #endregion

    #region 私有變數
    private IDamageable thisDamageable; //取得玩家腳本
    #endregion

    //生命週期
    #region 生命週期
    private void Awake() {
        thisDamageable = GetComponent<IDamageable>();
    }
    #endregion

    private void OnEnable() {
        GameEventSystem.Instance.Event_HpChanged += UpdateUIValue;            //監聽Enemy的血量變化

    }

    private void OnDisable() {
        GameEventSystem.Instance.Event_HpChanged -= UpdateUIValue;            //監聽Enemy的血量變化
    }

    // UI_BattlePlayerPanel綁定血條使用
    #region Bind(IDamageable damageable)
    public void Bind(IDamageable damageable) {
        thisDamageable = damageable;

        //  綁定的當下強制刷新一次
        if (thisDamageable is Player player)
        {
            int cur = player.Runtime.CurrentHp;
            int max = player.Runtime.MaxHp;
            UpdateUIValue(cur, max, thisDamageable);
        }
    }
    #endregion

    //Slidere更新text、Value方法
    #region UpdateUIValue(int currentValue, int maxValue,IDamageable damageable)
    private void UpdateUIValue(int currentValue, int maxValue, IDamageable damageable) {    //監聽Enemy上的ValueChanged事件，更新UISlider上的數值
        if (damageable != thisDamageable) return;

        if (slider != null)
        {
            slider.maxValue = maxValue; //  設定最大值
            slider.value = currentValue; //  更新當前值
        }

        if (text != null)
        {
            text.text = $"{title}: {currentValue} / {maxValue}"; // 更新顯示文字
        }
    }
    #endregion

}

