using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }
    public GameStateManager GameStateManager { get; private set; }
    public PlayerInputController PlayerInputController { get; private set; }

    [Header("全局角色材質")]
    public Material NormalMaterial;
    public Material FlashMaterial;

    public bool IsAllDataLoaded => IsPlayerDataLoaded && IsEnemyDataLoaded;
    public bool IsPlayerDataLoaded { get; private set; } = false;
    public bool IsEnemyDataLoaded { get; private set; } = false;

    private Dictionary<GameStateManager.GameState, IGameStateHandler> _GameStatehandlers;


    #region 生命週期
    private void Awake() {
        //單例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //GameStateManager
        GameStateManager = new GameStateManager();
        GameStateManager.OnStateEntered += HandleEnter;
        GameStateManager.OnStateExited += HandleExit;

        //找到 MonoBehaviour 並注入依賴
        PlayerInputController = FindObjectOfType<PlayerInputController>();
        if (PlayerInputController != null)
            PlayerInputController.Initialize(GameStateManager);
        else
            Debug.LogWarning("PlayerInputController not found in scene!");

        //初始化<GameState,IGameStatHandler>字典
        _GameStatehandlers = new Dictionary<GameStateManager.GameState, IGameStateHandler>{
            { GameStateManager.GameState.GameStart, new GameStartHandler(GameSceneManager.Instance, UIController_Input.Instance,PlayerStateManager.Instance) },
            { GameStateManager.GameState.Preparation, new PreparationHandler(PlayerStateManager.Instance, GameSceneManager.Instance, UIController_Input.Instance) },
            { GameStateManager.GameState.Battle, new BattleHandler(this, GameSceneManager.Instance, PlayerStateManager.Instance, PlayerInputController) },
            // EndGameHandler 之後再加
        };

    }

    private void OnDisable() {
        if (GameStateManager != null)
        {
            GameStateManager.OnStateEntered -= HandleEnter;
            GameStateManager.OnStateExited -= HandleExit;
        }
    }
    #endregion

    private IEnumerator Start() {
        yield return Addressables.InitializeAsync();

        yield return LoadPlayerStatsList();
        yield return LoadEnemyStatsList();
    }

    private void HandleEnter(GameStateManager.GameState state, string sceneName) {
        if (_GameStatehandlers.TryGetValue(state, out var handler))
            handler.Enter(sceneName);
        else
            Debug.LogWarning($"[GameSystem] No Enter handler found for state {state}");
    }
    private void HandleExit(GameStateManager.GameState state) {
        if (_GameStatehandlers.TryGetValue(state, out var handler))
            handler.Exit();
        else
            Debug.LogWarning($"[GameSystem] No Exit handler found for state {state}");
    }



    private IEnumerator LoadPlayerStatsList() {
        string address = "Assets/GameData/PlayerStatData.asset";
        AsyncOperationHandle<PlayerStatData> handle = Addressables.LoadAssetAsync<PlayerStatData>(address);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            PlayerStateManager.Instance.SetPlayerStatesDtny(handle.Result);
            IsPlayerDataLoaded = true;
        }
    }
    private IEnumerator LoadEnemyStatsList() {
        string address = "Assets/GameData/EnemyStatData.asset";
        AsyncOperationHandle<EnemyStatData> handle = Addressables.LoadAssetAsync<EnemyStatData>(address);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            EnemyStateManager.Instance.SetEnemyStatesDtny(handle.Result);
            IsEnemyDataLoaded = true;
        }

    }
    public IEnumerator WaitForDataReady() {
        // 持續等待，直到所有資料載入完成
        while (!IsAllDataLoaded)
        {
            yield return null; // 等待下一幀
        }
    }

}
