using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[DefaultExecutionOrder(0)]
public class UI_MainMenuController : UI_PanelBase
{
    public Button StatusPanelButton;
    public Button SkillsPanelButton;
    public Button EquipmentPanelButton;
    public Button SavePanelButton;

    public UI_PanelBase StatusPanel;
    public UI_PanelBase SkillsPanel;
    public UI_PanelBase EquipmentPanel;
    public UI_PanelBase SavePanel;

    //來自UIManager的堆疊
    private Stack<UI_PanelBase> _uiPanelsStack => UIManager.Instance.UiPanelsStack;
 
    private void OnEnable() {
        base.OnEnable();
        StatusPanelButton.onClick.AddListener(ShowStatusPanel);
        SkillsPanelButton.onClick.AddListener(ShowSkillUsagePanel);
        EquipmentPanelButton.onClick.AddListener(ShowEquipmentPanel);
        SavePanelButton.onClick.AddListener(ShowSavePanel);

    }

    private void OnDisable() {
        StatusPanelButton.onClick.RemoveListener(ShowStatusPanel);
        SkillsPanelButton.onClick.RemoveListener(ShowSkillUsagePanel);
        EquipmentPanelButton.onClick.RemoveListener(ShowEquipmentPanel);
        SavePanelButton.onClick.RemoveListener(ShowSavePanel);
    }


    public override void Refresh() {
    }

    //按鈕綁定
    public void ShowStatusPanel() => UIManager.Instance.ShowUIPanel(StatusPanel);
    public void ShowSkillUsagePanel() => UIManager.Instance.ShowUIPanel(SkillsPanel);
    public void ShowEquipmentPanel() => UIManager.Instance.ShowUIPanel(EquipmentPanel);
    public void ShowSavePanel() => UIManager.Instance.ShowUIPanel(SavePanel);


}
