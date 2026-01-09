using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

[DefaultExecutionOrder(-50)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private readonly List<UI_PanelBase> _allPanels = new List<UI_PanelBase>();
    public UI_InputController UIInputController => _uiInputController;

    [Header("UI物件")]
    public UI_StageClearController UI_StageClearController;
    public UI_PanelBase MenuPanel;
    public GameObject UI_Loading;
    public GameObject UI_SkillCooldownPanel;
    public UI_SkillSliderController UI_SkillSliderController;


    public Stack<UI_PanelBase> UiPanelsStack = new Stack<UI_PanelBase>(); // 儲存開啟中的 UI 面板
    private UI_InputController _uiInputController;


    #region 生命週期
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _uiInputController = GetComponent<UI_InputController>();
    }
    private void OnEnable() {
        _uiInputController.OnToggleMenuButton += OnToggleMenuButton;
    }
    private void OnDisable() {
        _uiInputController.OnToggleMenuButton -= OnToggleMenuButton;
    }
    private void Start() {
        foreach (var panel in _allPanels)
        {
            if (panel != null) panel.Hide();
        }

    }
    #endregion

    public void RegisterPanel(UI_PanelBase panel) {
        if (!_allPanels.Contains(panel)) {
            //Debug.Log($"{panel.name}註冊UIManager成功");
            _allPanels.Add(panel);
        }

    }

    public void SetLoadingUI(bool isOpen) {
        UI_Loading.SetActive(isOpen);
    }


    public void ShowUIPanel(UI_PanelBase panel) {
        if (panel == null) return;
        if (UiPanelsStack.Contains(panel))
        {
            return;
        }
        UiPanelsStack.Push(panel);
        if(panel!=null) panel.Show();
    }
    public void ShowMenuPanel() =>ShowUIPanel(MenuPanel);
    public void HideTopUIPanel() {
        if (UiPanelsStack.Count == 0) return;
        var topPanel = UiPanelsStack.Pop();
        if(topPanel!=null) topPanel.Hide();
    }
    public void HideAllUIPanels() {

        while (UiPanelsStack.Count > 0)
        {
            var panel = UiPanelsStack.Pop();
            if (panel != null)
            {
                panel.Hide();
            }
        }

        UI_SkillSliderController.gameObject.SetActive(false);
    }


    //事件方法
    public void OnToggleMenuButton() {
        if (UiPanelsStack.Count > 0) HideTopUIPanel();
        else ShowMenuPanel();
    } 
}
