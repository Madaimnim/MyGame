using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[DefaultExecutionOrder(0)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }      //單例
    public GameObject menuUIPanel;
    public GameObject statusUIPanel;
    public GameObject jobUIPanel;
    public GameObject equipmentUIPanel;
    public GameObject skillsUIPanel;
    public GameObject formationUIPanel;
    public Stack<GameObject> activeUIPanelsStack = new Stack<GameObject>(); // 儲存開啟中的 UI 面板
    public GameObject UI_Loading;

    private PlayerSystem _playerSystem => GameManager.Instance.PlayerSystem;
    private UIController_Skill uiSkillController;

    // 貫穿整個 UI 的核心變數
    public int currentPlayerId = -1;
    public Transform PlayerUiParent;

    #region 生命週期
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); 掛在主物件才需要

        uiSkillController = GetComponentInChildren<UIController_Skill>();
    }
    private void OnEnable() {
        _playerSystem.OnPlayerUnlocked += OnPlayerUnlocked;
        _playerSystem.OnPlayerSpawned += OnPlayerSpawned;
        CloseAllUIPanels();
    }
    private void OnDisable() {
        _playerSystem.OnPlayerUnlocked -= OnPlayerUnlocked;
        _playerSystem.OnPlayerSpawned -= OnPlayerSpawned;
    }
    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());

        UpdateUICurrentIndexAndPlayer(); // 初始化 UI
    }
    #endregion

    //事件綁定
    public void SetLoadingUI(bool isOpen) {
        UI_Loading.SetActive(isOpen);
    }
    private void OnPlayerSpawned(int id,PlayerStatsRuntime rt) {
        SpawnUIPlayer(id,rt);
    }

    private void OnPlayerUnlocked(int id) {
        if (currentPlayerId == -1)
        {
            currentPlayerId = id;
            UpdateUICurrentIndexAndPlayer();
        }
    }

    public GameObject SpawnUIPlayer(int id, PlayerStatsRuntime rt) {
        var factory = new DefaultPlayerFactory();
        var go = factory.CreatPlayer(rt, PlayerUiParent);
        if (go != null)
            rt.UiPlayerObject = go;
        return go;
    }

    //供Button訂閱傳入「string 動畫名稱」，執行當前角色的動畫撥放
    public void PlayUIAttackAnimation(string animationName) {
        _playerSystem.PlayerStatsRuntimes[currentPlayerId].UiPlayerObject.
            GetComponent<Animator>()?.Play(Animator.StringToHash(animationName));
    }

    public void OpenUIPanel(GameObject panel) {
        if (panel == null)
        {
            Debug.LogError(" OpenUIPanel: panel 為 null，請確認 Inspector 設定！");
            return;
        }

        if (activeUIPanelsStack.Count > 0 && activeUIPanelsStack.Contains(panel))
        {
            Debug.LogWarning($" {panel.name} 已經在堆疊中，不重複添加！");
            return;
        }
        activeUIPanelsStack.Push(panel);
        panel.SetActive(true);
    }
    public void OpenUIMene() {
        OpenUIPanel(menuUIPanel);
    }
    public void OpenUIStatus() {
        OpenUIPanel(statusUIPanel);
    }
    public void OpenUISkillUsage() {
        OpenUIPanel(skillsUIPanel);
    }
    public void CloseTopUIPanel() {
        if (activeUIPanelsStack.Count == 0)
        {
            Debug.LogWarning("⚠️ 嘗試關閉 UI 但 Stack 為空！");
            return;
        }
        GameObject topPanel = activeUIPanelsStack.Pop();
        if (topPanel != null)
            topPanel.SetActive(false);
    }
    public void CloseAllUIPanels() {
        menuUIPanel.SetActive(false);
        statusUIPanel.SetActive(false);
        //jobUIPanel.SetActive(false);
        //equipmentUIPanel.SetActive(false);
        skillsUIPanel.SetActive(false);
        //formationUIPanel.SetActive(false);
        activeUIPanelsStack.Clear();
    }

    //更新UI裡的腳色ID
    public void UpdateUICurrentIndexAndPlayer() {
        var unlockedPlayerIDs = _playerSystem.UnlockedIds.ToList();
        if (unlockedPlayerIDs.Count == 0)   return;
 
        if (!unlockedPlayerIDs.Contains(currentPlayerId))
            currentPlayerId = unlockedPlayerIDs.First();
        if (_playerSystem.TryGetStatsRuntime(currentPlayerId, out var newPlayer))
        {
            EventBus.Trigger(new UICurrentPlayerChangEvent(newPlayer));
        }
        else
        {
            Debug.LogError($"UpdateUICrrentIndexAndPlayer: 找不到 ID={currentPlayerId} 的玩家狀態");
        }
    }

    //提供外部方法變更currentIndex
    public void ChangCurrentPlayerID(int AddNumber) {
        var unlockedPlayerIDs = _playerSystem.UnlockedIds.ToList();
        if (unlockedPlayerIDs.Count == 0) return;

        int currentIndex = unlockedPlayerIDs.IndexOf(currentPlayerId);
        if (currentIndex == -1) currentIndex = 0; // 找不到就回第一個

        int newIndex = (currentIndex + AddNumber + unlockedPlayerIDs.Count) % unlockedPlayerIDs.Count;
        currentPlayerId = unlockedPlayerIDs[newIndex];

        UpdateUICurrentIndexAndPlayer();
        uiSkillController.skillSelectionPanel.SetActive(false);
    }

}
