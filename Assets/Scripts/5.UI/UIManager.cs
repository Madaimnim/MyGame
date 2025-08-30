using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }      //單例
    public GameObject menuUIPanel;
    public GameObject statusUIPanel;
    public GameObject jobUIPanel;
    public GameObject equipmentUIPanel;
    public GameObject skillsUIPanel;
    public GameObject formationUIPanel;
    public Dictionary<int, GameObject> activeUIPlayersDtny = new Dictionary<int, GameObject>();
    public Stack<GameObject> activeUIPanelsStack = new Stack<GameObject>(); // 儲存開啟中的 UI 面板

    private UISkillController uiSkillController;
    #region 角色管理
    public int currentPlayerId = 1;   // 貫穿整個 UI 的核心變數
    #endregion

    //生命週期
    #region 生命週期
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        uiSkillController = GetComponentInChildren<UISkillController>();
    }

    private void OnEnable() {
        CloseAllUIPanels();
    }

    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        UpdateUICrrentIndexAndPlayer(); // ✅ 初始化 UI
    }
    #endregion

    //供Button訂閱傳入「string 動畫名稱」，執行當前角色的動畫撥放
    #region PlayUIAttackAnimation(string animationName)
    public void PlayUIAttackAnimation(string animationName) {
        activeUIPlayersDtny[currentPlayerId].GetComponent<Animator>().Play(Animator.StringToHash(animationName));
    }
    #endregion

    //開起UI方法
    #region OpenUIPanel(GameObject panel)
    public void OpenUIPanel(GameObject panel) {
        if (panel == null)
        {
            Debug.LogError("❌ OpenUIPanel: panel 為 null，請確認 Inspector 設定！");
            return;
        }

        if (activeUIPanelsStack.Count > 0 && activeUIPanelsStack.Contains(panel))
        {
            Debug.LogWarning($"⚠️ {panel.name} 已經在堆疊中，不重複添加！");
            return;
        }
        activeUIPanelsStack.Push(panel);
        panel.SetActive(true);
    }
    #endregion

    //開啟UI菜單
    #region
    public void OpenUIMene() {
        OpenUIPanel(menuUIPanel);
    }
    #endregion

    //開啟UI狀態
    #region
    public void OpenUIStatus() {
        OpenUIPanel(statusUIPanel);
    }
    #endregion

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
        //skillsUIPanel.SetActive(false);
        //formationUIPanel.SetActive(false);
        activeUIPanelsStack.Clear();
    }

    //更新UI裡的腳色ID
    #region UpdateUICrrentIndexAndPlayer()
    public void UpdateUICrrentIndexAndPlayer() {
        var unlockedPlayerIDs = PlayerStateManager.Instance.unlockedPlayerIDsHashSet;

        if (unlockedPlayerIDs.Count == 0)
        {
            return;
        }

        // 確保 currentIndex 在合理範圍
        currentPlayerId = Mathf.Clamp(currentPlayerId, 0, unlockedPlayerIDs.Count);
        PlayerStateManager.PlayerStatsRuntime newPlayer = PlayerStateManager.Instance.playerStatesDtny[currentPlayerId];

        EventBus.Trigger(new UICurrentPlayerChangEvent(PlayerStateManager.Instance.playerStatesDtny[currentPlayerId]));
    }
    #endregion

    //提供外部方法變更currentIndex
    #region ChangCurrentPlayerID(int AddNumber)
    public void ChangCurrentPlayerID(int AddNumber) {
        var unlockedPlayerIDs = PlayerStateManager.Instance.unlockedPlayerIDsHashSet;

        if (unlockedPlayerIDs.Count == 0) return;

        currentPlayerId = (currentPlayerId - 1 + AddNumber + unlockedPlayerIDs.Count) % unlockedPlayerIDs.Count + 1;
        UpdateUICrrentIndexAndPlayer();

        uiSkillController.skillSelectionPanel.SetActive(false);
    }

    public void SetCurrentPlayer(int newPlayerID) {
        var unlockedPlayerIDs = PlayerStateManager.Instance.unlockedPlayerIDsHashSet;

        if (unlockedPlayerIDs.Count == 0) return;
        if (newPlayerID < 1 || newPlayerID > unlockedPlayerIDs.Count) return;

        currentPlayerId = newPlayerID;
        UpdateUICrrentIndexAndPlayer();
    }

    #endregion

    //打開Menu
    #region

    #endregion
}
