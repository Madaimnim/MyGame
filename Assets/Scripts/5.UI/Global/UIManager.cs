using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[DefaultExecutionOrder(-50)]
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
    public GameObject UI_Loading;

    private UIController_Skill uiSkillController;
    #region 角色管理
    public int currentPlayerId = -1;   // 貫穿整個 UI 的核心變數
    #endregion

    //生命週期
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
        CloseAllUIPanels();
    }

    private IEnumerator Start() {
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());

        if (PlayerStateManager.Instance.UnlockedPlayerIDsHashSet.Count > 0)
        {
            currentPlayerId = PlayerStateManager.Instance.UnlockedPlayerIDsHashSet.First(); // 真實 ID
        }

        UpdateUICrrentIndexAndPlayer(); // 初始化 UI
    }
    #endregion

    public void SetLoadingUI(bool isOpen) {
        UI_Loading.SetActive(isOpen);
    }


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


    //開啟UI技能
    #region
    public void OpenUISkillUsage() {
        OpenUIPanel(skillsUIPanel);
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
        skillsUIPanel.SetActive(false);
        //formationUIPanel.SetActive(false);
        activeUIPanelsStack.Clear();
    }

    //更新UI裡的腳色ID
    #region UpdateUICrrentIndexAndPlayer()
    public void UpdateUICrrentIndexAndPlayer() {
        var unlockedPlayerIDs = PlayerStateManager.Instance.UnlockedPlayerIDsHashSet;

        if (unlockedPlayerIDs.Count == 0)   return;
        // 如果 currentPlayerId 不在已解鎖清單，就指定第一個
        if (!unlockedPlayerIDs.Contains(currentPlayerId))
            currentPlayerId = unlockedPlayerIDs.First();
        if (PlayerStateManager.Instance.TryGetState(currentPlayerId, out var newPlayer))
        {
            EventBus.Trigger(new UICurrentPlayerChangEvent(newPlayer));
        }
        else
        {
            Debug.LogError($"❌ UpdateUICrrentIndexAndPlayer: 找不到 ID={currentPlayerId} 的玩家狀態");
        }
    }
    #endregion

    //提供外部方法變更currentIndex
    #region ChangCurrentPlayerID(int AddNumber)
    public void ChangCurrentPlayerID(int AddNumber) {
        var unlockedPlayerIDs = PlayerStateManager.Instance.UnlockedPlayerIDsHashSet.ToList();

        if (unlockedPlayerIDs.Count == 0) return;

        // 取得目前 ID 在列表的索引
        int currentIndex = unlockedPlayerIDs.IndexOf(currentPlayerId);
        if (currentIndex == -1) currentIndex = 0; // 找不到就回第一個

        // 算新的索引
        int newIndex = (currentIndex + AddNumber + unlockedPlayerIDs.Count) % unlockedPlayerIDs.Count;

        // **直接把真實的玩家 ID 存回 currentPlayerId**
        currentPlayerId = unlockedPlayerIDs[newIndex];

        Debug.Log($"✅ 切換角色 → 真實 ID = {currentPlayerId}");

        UpdateUICrrentIndexAndPlayer();
        uiSkillController.skillSelectionPanel.SetActive(false);
    }

    #endregion

    //打開Menu
    #region

    #endregion
}
