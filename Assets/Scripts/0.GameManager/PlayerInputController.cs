using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using System;

public class PlayerInputController :SubSystemBase,IInputProvider
{
    private GameObject _selectionIndicator;  // 選框箭頭

    private Dictionary<int, GameObject> _deployedPlayersGameObjectDtny; 
    private List<int> _deployedPlayerIDsList ; // 存放目前可用的角色 ID
    private int currentPlayerIndex = -1;  // 當前選擇的角色索引

    private CinemachineVirtualCamera _virtualCam;
    private GameStateSystem _gameStateSystem;

    private bool _canControl = false;

    public PlayerInputController(GameManager gm) :base(gm){
        _deployedPlayerIDsList = new List<int>();
        _deployedPlayersGameObjectDtny = new Dictionary<int, GameObject>();
    }

  

    public override void Initialize() {
        _gameStateSystem = GameManager.GameStateSystem;
        _virtualCam = UnityEngine.Object.FindObjectOfType<CinemachineVirtualCamera>();
        _gameStateSystem.OnControlEnabledChanged += OnControlEnabledChanged;
    }
    public override void Update(float deltaTime) {
        if (!_canControl) return;
        HandlePlayerSwitch();
    }
    public override void Shutdown() {
        if (_gameStateSystem != null)
            _gameStateSystem.OnControlEnabledChanged -= OnControlEnabledChanged;
    }

    //IInputProvider
    public Vector2 GetMoveDirection() {
        if (_canControl)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            return new Vector2(moveX, moveY).normalized;
        }
        else
            return Vector2.zero;

    }
    public bool IsAttackPressed() {
        if (_canControl)
            return Input.GetKeyDown(KeyCode.Space);
        else
            return false;
    }
    public void OnControlEnabledChanged(bool canControl) {
        _canControl = canControl;
        Debug.Log($"[PlayerInputController] OnControlEnabledChanged={canControl}");
    }

 

    //初始化角色清單，並預選第一個角色
    public void InitailPlayerList() {
        UpdatePlayerList(); // 初始化角色列表
        if (_deployedPlayerIDsList.Count > 0)
        {
            SelectPlayer(0);  // 預設選擇第一個角色
        }
    }

    // 更新玩家列表
    private void UpdatePlayerList() {
        _deployedPlayersGameObjectDtny = PlayerStateManager.Instance.DeployedPlayersGameObjectDtny; // 直接引用 Dictionary
        _deployedPlayerIDsList = new List<int>(_deployedPlayersGameObjectDtny.Keys); // 取得所有可用的角色 ID

        if (_deployedPlayerIDsList.Count == 0)
        {
            Debug.LogWarning("目前沒有可控制的角色！");
        }
    }

    // 處理角色切換
    private void HandlePlayerSwitch() {
        // 數字鍵 1-9 切換角色（根據當前角色數量自適應）
        for (int i = 0; i < _deployedPlayerIDsList.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectPlayer(i);
                return;
            }
        }

        // 滑鼠點擊角色切換
        if (Input.GetMouseButtonDown(0))
        {
            // 這裡只是 Debug UI 狀態，不再 return
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("點擊到 UI");
            }

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 只檢測 Player 層
            int layerMask = LayerMask.GetMask("Player");

            // 用 OverlapPointAll 抓取所有 Collider
            Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos, layerMask);

            if (hits.Length == 0)
            {
                //Debug.Log($"沒有命中任何 Player 層物件");
            }
            else
            {
                foreach (var hit in hits)
                {
                    Debug.Log($"命中物件: {hit.gameObject.name}, Layer: {LayerMask.LayerToName(hit.gameObject.layer)}");

                    // 嘗試匹配 _deployedPlayersGameObjectDtny
                    foreach (var kvp in _deployedPlayersGameObjectDtny)
                    {
                        if (hit.gameObject == kvp.Value)
                        {
                            int index = _deployedPlayerIDsList.IndexOf(kvp.Key);
                            if (index != -1)
                            {
                                SelectPlayer(index);
                            }
                        }
                    }
                }
            }
        }
    }

    // 選擇角色、指標跟隨、相機跟隨
    private void SelectPlayer(int index) {
        if (index < 0 || index >= _deployedPlayerIDsList.Count) return;

        currentPlayerIndex = index;

        // 選中的 → 玩家控制
        int selectedId = _deployedPlayerIDsList[index];
        var selectedPlayer = _deployedPlayersGameObjectDtny[selectedId].GetComponent<Player>();
        selectedPlayer.SetInputProvider(this); // 指定玩家輸入控制

        // 其他角色 → AI 控制
        foreach (var kvp in _deployedPlayersGameObjectDtny)
        {
            if (kvp.Key == selectedId) continue; // 略過選中的
            var p = kvp.Value.GetComponent<Player>();
            p.SetInputProvider(p.GetComponent<PlayerAI>()); // 指定 AI 控制
        }

        UpdateSelectionIndicator();
        CameraManager.Instance.Follow(selectedPlayer.transform);
    }

    // 更新指標的位置
    private void UpdateSelectionIndicator() {
        if (!_deployedPlayersGameObjectDtny.ContainsKey(_deployedPlayerIDsList[currentPlayerIndex]))
        {
            Debug.LogWarning("玩家 ID 不存在於 _deployedPlayersGameObjectDtny！");
            return;
        }
        int playerID = _deployedPlayerIDsList[currentPlayerIndex];
        Player player = PlayerStateManager.Instance.GetBattlePlayerObject(playerID).GetComponent<Player>();

        if (player == null) { Debug.LogWarning("取得的 Player 為 null！"); return; }
        if (player.SelectIndicatorPoint == null) { Debug.LogWarning("Player 的 selectIndicatorPoint 為 null！"); return; }

        Transform indicatorPoint = player.SelectIndicatorPoint.transform;
        Debug.LogWarning($"預置_selectionIndicator為:{_selectionIndicator!=null}");
        if(_selectionIndicator==null)_selectionIndicator = UnityEngine.Object.Instantiate(GameManager.PrefabConfig.SelectionIndicatorPrefab);
        Debug.LogWarning($"設定後_selectionIndicator為:{_selectionIndicator != null}");
        _selectionIndicator.transform.SetParent(indicatorPoint);
        _selectionIndicator.transform.localPosition = Vector3.zero;
    }

}
