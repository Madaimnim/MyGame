using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using System;
using System.Linq;

public class PlayerInputController :SubSystemBase,IInputProvider
{
    private GameObject _selectionIndicator;  // 選框箭頭

    private int _currentPlayerId = -1;               //當前選擇的角色ID
    private int _currentPlayerIndex = -1;            //當前選擇的角色索引

    private GameStateSystem _gameStateSystem => GameManager.GameStateSystem;
    private PlayerSystem _playerSystem => GameManager.PlayerSystem;
    private bool _canControl = false;

    public event Action<Transform> OnBattlePlayerSelected;
    public PlayerInputController(GameManager gm) : base(gm) { }

    public override void Initialize() {
        _gameStateSystem.OnPlayerCanControlChanged += OnPlayerCanControlChanged;
    }
    public override void Update(float deltaTime) {
        if (!_canControl) return;
        HandlePlayerSwitch();
    }
    public override void Shutdown() {
        if (_gameStateSystem != null)
            _gameStateSystem.OnPlayerCanControlChanged -= OnPlayerCanControlChanged;
    }

    //IInputProvider
    public Vector2 GetMoveDirection() {
        if (_canControl)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            return new Vector2(moveX, moveY).normalized;
        }
        return Vector2.zero;
    }
    public bool IsAttackPressed() {
        return _canControl && Input.GetKeyDown(KeyCode.Space);
    }

    public void OnPlayerCanControlChanged(bool canControl) {
        _canControl = canControl;
    }

 
    //初始化角色清單，並預選第一個角色
    public void SelectDefaultPlayer() {
        var ids = _playerSystem.BattleObjects.Keys.ToList();
        if (ids.Count > 0) SelectPlayer(0); 
        else Debug.LogWarning("目前沒有可控制的角色！");
    }
    // 處理角色切換
    private void HandlePlayerSwitch() {
        var ids = _playerSystem.BattleObjects.Keys.ToList();

        // 數字鍵 1-9 切換角色
        for (int i = 0; i < ids.Count && i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectPlayer(i);
                return;
            }

        // 滑鼠點擊角色切換
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) Debug.Log("點擊到 UI");

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int layerMask = LayerMask.GetMask("Player");
            Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos, layerMask);

            foreach (var hit in hits)
                foreach (var kvp in _playerSystem.BattleObjects)
                    if (hit.gameObject == kvp.Value)
                    {
                        int index = ids.IndexOf(kvp.Key);
                        if (index != -1)
                        {
                            SelectPlayer(index);
                            return;
                        }
                    }
        }
    }

    // 選擇角色、指標跟隨
    private void SelectPlayer(int index) {
        var ids = _playerSystem.BattleObjects.Keys.ToList();
        if (index < 0 || index >= ids.Count) return;
        _currentPlayerIndex = index;
        _currentPlayerId = ids[_currentPlayerIndex];

        // 選中的 → 玩家控制
        var selectedPlayer = _playerSystem.BattleObjects[_currentPlayerId].GetComponent<Player>();
        selectedPlayer?.SetInputProvider(this);
        // 其他角色 → AI 控制
        foreach (var kvp in _playerSystem.BattleObjects)
        {
            if (kvp.Key == _currentPlayerId) continue;
            var p = kvp.Value.GetComponent<Player>();
            p?.SetInputProvider(p.GetComponent<PlayerAI>());
        }

        UpdateSelectionIndicator(selectedPlayer);
        if (selectedPlayer != null) OnBattlePlayerSelected?.Invoke(selectedPlayer.transform);
    }

    // 更新指標的位置
    private void UpdateSelectionIndicator(Player player) {
        if (player == null || player.SelectIndicatorPoint == null)
        {
            Debug.LogWarning("Player 或 SelectIndicatorPoint 為 null！");
            return;
        }

        if (_selectionIndicator == null)
            _selectionIndicator = UnityEngine.Object.Instantiate(GameManager.PrefabConfig.SelectionIndicatorPrefab);

        _selectionIndicator.transform.SetParent(player.SelectIndicatorPoint.transform);
        _selectionIndicator.transform.localPosition = Vector3.zero;
    }

}
