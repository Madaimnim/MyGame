using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using System;
using System.Linq;

public class PlayerInputController : SubSystemBase, IInputProvider
{
    private GameObject _selectionIndicator;          //選框箭頭
    private GameObject _moveMarkerInstance;          //點擊地板特效

    private Player _currentPlayer;
    private bool _canControl = false;

    public event Action<Transform> OnBattlePlayerSelected;
    public PlayerInputController(GameManager gm) : base(gm) {}

    public override void Initialize() {
        _selectionIndicator = UnityEngine.Object.Instantiate(GameManager.PrefabConfig.SelectionIndicatorPrefab);
        _selectionIndicator.SetActive(false);
        GameManager.GameStateSystem.OnPlayerCanControlChanged += OnPlayerCanControlChanged;
    }
    public override void Update(float deltaTime) {
        if (!_canControl) return;
        ClickPlayer();

        if (_currentPlayer == null) return;
        if (_currentPlayer.InputProvider != this) return;
        SetIntentMoveDirection();
        SetIntentSkillSlot();
    }

    public void OnPlayerCanControlChanged(bool canControl) => _canControl = canControl;
    //-------------------------------選取腳色--------------------------------------------------------------------------------
    private void ClickPlayer() {
        if (!Input.GetMouseButtonDown(0)) return;
        if (EventSystem.current.IsPointerOverGameObject()) Debug.Log("點擊到 UI");

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int layerMask = LayerMask.GetMask("Player");
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layerMask);

        if (hit == null) return;
        var player = hit.GetComponent<Player>();
        SelectPlayer(player);
    }
    public void SelectPlayer(Player selectedPlayer) {
        _currentPlayer = selectedPlayer;
        selectedPlayer.SetInputProvider(this);
        foreach (var kvp in PlayerUtility.AllPlayers)
        {
            var player = kvp.Value;
            if (player == selectedPlayer) continue;
            player.SetInputProvider(player.AIComponent);
        }

        SetSelectionIndicatorParent(selectedPlayer);
        if (selectedPlayer != null) OnBattlePlayerSelected?.Invoke(selectedPlayer.transform);
    }
    private void SetSelectionIndicatorParent(Player player) {
        _selectionIndicator.SetActive(true);
        _selectionIndicator.transform.SetParent(player.SelectIndicatorPoint.transform);
        _selectionIndicator.transform.localPosition = Vector3.zero;
    }
    
    //-------------------------------Intent設定--------------------------------------------------------------------------------
    public void SetIntentMoveDirection() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0)
        {
            _currentPlayer.MoveComponent.IntentTargetPosition = null; // 取消自動移動
            _currentPlayer.MoveComponent.IntentDirection = new Vector2(moveX, moveY).normalized;
            return;
        }

        // 滑鼠右鍵點地板：設定新目標位置
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _currentPlayer.MoveComponent.IntentTargetPosition = mouseWorldPos;
            
            VFXManager.Instance.Play("ClickGround01", mouseWorldPos);
        }
    }
    public void SetIntentSkillSlot() {
        for (int i = 0; i < _currentPlayer.SkillComponent.SkillSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _currentPlayer.SkillComponent.IntentSkillSlot = i;

                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _currentPlayer.SkillComponent.IntentTargetPosition = mouseWorldPos;

                break;
            }
        }
    }
    public void SetIntentTargetPosition() {}
}
