using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //Config
    public PrefabConfig PrefabConfig => _prefabConfig;
    [SerializeField] private PrefabConfig _prefabConfig;
    public SceneConfig SceneConfig => _sceneConfig;
    [SerializeField] private SceneConfig _sceneConfig;

    public Transform PlayerBattleParent;
    public Transform PlayerUiParent;
    public Vector3 PlayerSpawnPosition;

    public Dictionary<GameStateSystem.GameState,IGameStateHandler> GameStateHandlers { get; private set; }

    //子系統
    public GameStateSystem GameStateSystem { get; private set; }
    public GameSceneSystem GameSceneSystem { get;private set; }
    public PlayerInputController PlayerInputController { get; private set; }
    public PlayerSystem PlayerSystem { get; private set; }

    public bool IsAllDataLoaded => IsPlayerDataLoaded && IsEnemyDataLoaded;
    public bool IsPlayerDataLoaded { get; private set; } = false;
    public bool IsEnemyDataLoaded { get; private set; } = false;


    private readonly List<SubSystemBase> _subSystems = new();
    private GameStateRouter _gameStateRouter;

    private void Awake() {
        //單例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        UIManager.Instance.SetLoadingUI(true);

        //建子系統
        GameStateSystem = new GameStateSystem(this);
        GameSceneSystem = new GameSceneSystem(this);
        PlayerInputController = new PlayerInputController(this);
        _gameStateRouter = new GameStateRouter(this);
        PlayerSystem = new PlayerSystem(this);

        //建 Handler map並建構子給_gameStateRouter
        var runner = new CoroutineRunnerAdapter(this);
        GameStateHandlers = new Dictionary<GameStateSystem.GameState, IGameStateHandler>{
            { GameStateSystem.GameState.GameStart, new GameStartHandler(runner,GameSceneSystem, UIController_Input.Instance,PlayerSystem) },
            { GameStateSystem.GameState.Preparation, new PreparationHandler(PlayerSystem, GameSceneSystem, UIController_Input.Instance) },
            { GameStateSystem.GameState.Battle, new BattleHandler( GameSceneSystem, PlayerSystem, PlayerInputController) },
        };


        //所有子系統初始化
        foreach (var sub in _subSystems)  sub.Initialize();
    }
    private void OnDisable() { }
    private IEnumerator Start() {
        yield return Addressables.InitializeAsync();
        yield return LoadPlayerStatsList();
        yield return LoadEnemyStatsList();

        // 遊戲一開始 → 自動切到 StartScene
        GameSceneSystem.LoadSceneByKey("Start");
    }
    private void Update() {
        float dt = Time.deltaTime;
        foreach (var sub in _subSystems)  sub.Update(dt); 
    }

    public void RegisterSubsystem(SubSystemBase subSystem) {
        if (!_subSystems.Contains(subSystem))
            _subSystems.Add(subSystem);
    }


    private IEnumerator LoadPlayerStatsList() {
        string address = "PlayerStatData";
        AsyncOperationHandle<PlayerStatData> handle = Addressables.LoadAssetAsync<PlayerStatData>(address);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            PlayerSystem.SetPlayerStatsRuntimes(handle.Result);
            IsPlayerDataLoaded = true;
        }
    }
    private IEnumerator LoadEnemyStatsList() {
        string address = "EnemyStatData";
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
