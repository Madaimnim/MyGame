using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameStateSystem GameStateSystem { get; private set; }
    public PlayerStateManager PlayerStateManager { get; private set; }


    public PlayerInputController PlayerInputController { get; private set; }

    [Header("全局角色材質")]
    public Material NormalMaterial;
    public Material FlashMaterial;

    public bool IsAllDataLoaded => IsPlayerDataLoaded && IsEnemyDataLoaded;
    public bool IsPlayerDataLoaded { get; private set; } = false;
    public bool IsEnemyDataLoaded { get; private set; } = false;

    private readonly List<SubSystemBase> _subSystems = new();
    private GameStateRouter _gameStateRouter;

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

        //建子系統
        GameStateSystem = new GameStateSystem(this);

        //找到 MonoBehaviour 並注入依賴
        PlayerInputController = FindObjectOfType<PlayerInputController>();
        if (PlayerInputController != null)
            PlayerInputController.Initialize(GameStateSystem);
        else
            Debug.LogWarning("PlayerInputController not found in scene!");

        //1.建立服務(轉接器包裝)
        var sceneServiceAdapter = new SceneServiceAdapter();
        var uiInputSetviceAdapter = new UIInputServiceAdapter();
        var playerRosterAdapter = new PlayerRosterAdapter();
        var gameEventSAdapter = new GameEventsAdapter();

        //2.建 Handler map並建構子給_gameStateRouter
        var runner = new CoroutineRunnerAdapter(this);

        var handlers = new Dictionary<GameStateSystem.GameState, IGameStateHandler>{
            { GameStateSystem.GameState.GameStart, new GameStartHandler(GameSceneManager.Instance, UIController_Input.Instance,PlayerStateManager.Instance) },
            { GameStateSystem.GameState.Preparation, new PreparationHandler(PlayerStateManager.Instance, GameSceneManager.Instance, UIController_Input.Instance) },
            { GameStateSystem.GameState.Battle, new BattleHandler(runner, GameSceneManager.Instance, PlayerStateManager.Instance, PlayerInputController) },
            // EndGameHandler 之後再加
        };
        _gameStateRouter = new GameStateRouter(GameStateSystem,handlers);

        //所有子系統初始化
        foreach (var sub in _subSystems)  sub.Initialize();
    }
    private void OnDisable() { }

    private IEnumerator Start() {
        yield return Addressables.InitializeAsync();

        yield return LoadPlayerStatsList();
        yield return LoadEnemyStatsList();
    }

    private void Update() {
        float dt = Time.deltaTime;
        foreach (var sub in _subSystems)  sub.Update(dt); 
    }

    #endregion

    public void RegisterSubsystem(SubSystemBase subSystem) {
        if (!_subSystems.Contains(subSystem))
            _subSystems.Add(subSystem);
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
