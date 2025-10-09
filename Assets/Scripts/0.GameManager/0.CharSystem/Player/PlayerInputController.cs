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
        _selectionIndicator = UnityEngine.Object.Instantiate(GameManager.PrefabConfig.SelectionIndicatorPrefab,GameManager.Instance.PlayerBattleParent);
        _selectionIndicator.SetActive(false);
        GameManager.GameStateSystem.OnPlayerCanControlChanged += OnPlayerCanControlChanged;
    }
    public override void Update(float deltaTime) {
        if (!_canControl) return;
        ClickPlayer();

        if (_currentPlayer == null) return;
        if (_currentPlayer.InputProvider != this) return;

        HandleMoveInput();    
        HandleSkillInput();    
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
        ResetIntent(selectedPlayer.MoveComponent, selectedPlayer.SkillComponent);

        foreach (var kvp in PlayerUtility.AllPlayers)
        {
            var player = kvp.Value;
            if (player == selectedPlayer) continue;
            player.SetInputProvider(player.AIComponent);
        }

        SetSelectionIndicatorParent(selectedPlayer);
        CameraManager.Instance.Follow(_currentPlayer.transform);
        if (selectedPlayer != null) OnBattlePlayerSelected?.Invoke(selectedPlayer.transform);
    }
    private void SetSelectionIndicatorParent(Player player) {
        _selectionIndicator.SetActive(true);
        _selectionIndicator.transform.SetParent(player.SelectIndicatorPoint.transform);
        _selectionIndicator.transform.localPosition = Vector3.zero;
    }
    //-------------------------------Input輸入--------------------------------------------------------------------------------
    private void HandleMoveInput() {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 有方向鍵輸入 → 優先使用方向移動
        if (moveX != 0 || moveY != 0)
        {
            Vector2 dir = new Vector2(moveX, moveY).normalized;
            SetIntentMove(_currentPlayer.MoveComponent,direction: dir);                     
            return;
        }
        // 若方向鍵放開，且沒有地板移動目標 → 停止移動
        if (!_currentPlayer.MoveComponent.IntentTargetPosition.HasValue)
            SetIntentMove(_currentPlayer.MoveComponent, direction: Vector2.zero);            
        // 滑鼠右鍵：點擊地板移動
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            SetIntentMove(_currentPlayer.MoveComponent, targetPosition: mouseWorldPos);      
            VFXManager.Instance.Play("ClickGround01", mouseWorldPos);
        }
    }
    private void HandleSkillInput() {
        for (int i = 0; i < _currentPlayer.SkillComponent.SkillSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, LayerMask.GetMask("Enemy"));

                Transform targetTransform = null;
                if (hit != null && hit.TryGetComponent(out Enemy enemy))
                    targetTransform = enemy.transform;

                SetIntentSkill(_currentPlayer.SkillComponent, i, targetPosition: mouseWorldPos, targetTransform: targetTransform);
                break;
            }
        }
    }

    //-------------------------------Intent設定--------------------------------------------------------------------------------
    public void ResetIntent(MoveComponent moveComponent,SkillComponent skillComponent) {
        SetIntentMove(moveComponent);
        SetIntentSkill(skillComponent, -1);
    }
    public void SetIntentMove(MoveComponent moveComponent,Vector2? direction = null, Vector2? targetPosition = null, Transform targetTransform = null) {
        moveComponent.IntentTargetTransform = targetTransform;
        moveComponent.IntentTargetPosition = targetPosition;
        moveComponent.IntentDirection = direction ?? Vector2.zero;
    }
    public void SetIntentSkill(SkillComponent skillComponent,int slotIndex, Vector2? targetPosition = null, Transform targetTransform = null) {
        skillComponent.IntentSlotIndex = slotIndex;
        skillComponent.IntentTargetTransform = targetTransform;
        skillComponent.IntentTargetPosition = targetPosition ?? Vector2.zero;
    }
}
