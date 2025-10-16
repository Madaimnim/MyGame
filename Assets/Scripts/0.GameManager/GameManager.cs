using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using JetBrains.Annotations;

[DefaultExecutionOrder(-50)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    //Config
    public PrefabConfig PrefabConfig => _prefabConfig;
    [SerializeField] private PrefabConfig _prefabConfig;
    public SceneConfig SceneConfig => _sceneConfig;
    [SerializeField] private SceneConfig _sceneConfig;

    public Transform PlayerBattleParent;
    public Transform EnemyBattleParent;
    public LineRenderer LineRenderer;

    public Dictionary<GameStateSystem.GameState,IGameStateHandler> GameStateHandlers { get; private set; }

    //子系統
    public GameStateSystem GameStateSystem { get; private set; }
    public GameSceneSystem GameSceneSystem { get;private set; }
    public PlayerInputController PlayerInputController { get; private set; }
    public PlayerStateSystem PlayerStateSystem { get; private set; }
    public EnemyStateSystem EnemyStateSystem { get; private set; }

    public bool IsAllDataLoaded { get; private set;} = false;

    //事件
    public event Action OnAllSubSystemReady;
    public event Action OnAllDataLoaded;

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

        //建子系統
        GameStateSystem = new GameStateSystem(this);
        GameSceneSystem = new GameSceneSystem(this);
        PlayerInputController = new PlayerInputController(this);
        _gameStateRouter = new GameStateRouter(this);
        PlayerStateSystem = new PlayerStateSystem(this);
        EnemyStateSystem = new EnemyStateSystem(this);

        //建 Handler map並建構子給_gameStateRouter
        var runner = new CoroutineRunnerAdapter(this);
        GameStateHandlers = new Dictionary<GameStateSystem.GameState, IGameStateHandler>{
            { GameStateSystem.GameState.GameStart, new GameStartHandler(runner,GameSceneSystem, PlayerStateSystem) },
            { GameStateSystem.GameState.Preparation, new PreparationHandler( GameSceneSystem) },
            { GameStateSystem.GameState.Battle, new BattleHandler( GameSceneSystem, PlayerStateSystem, PlayerInputController) },
        };

        //所有子系統初始化
        foreach (var sub in _subSystems)  sub.Initialize();
        
        //發事件
        OnAllSubSystemReady?.Invoke();
    }
    private void OnEnable() {}
    private void OnDisable() {}

    private IEnumerator Start() {
        yield return Addressables.InitializeAsync();
        yield return LoadPlayerStatsList();
        yield return LoadEnemyStatsList();
        
        IsAllDataLoaded = true;
        OnAllDataLoaded?.Invoke();

        // 遊戲一開始 → 自動切到 StartScene
        GameSceneSystem.LoadSceneByKey("Start");
    }
    private void Update() {
        float dt = Time.deltaTime;
        foreach (var sub in _subSystems)  sub.Update(dt); 
    }

    //繼承SubSystemBase的子系統，建構子時自動訂閱
    public void RegisterSubsystem(SubSystemBase subSystem) {
        if (!_subSystems.Contains(subSystem))
            _subSystems.Add(subSystem);
    }

    private IEnumerator LoadPlayerStatsList() {
        string address = "PlayerStatData";
        AsyncOperationHandle<PlayerStatData> handle = Addressables.LoadAssetAsync<PlayerStatData>(address);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            PlayerStateSystem.SetPlayerStatsRuntimeDtny(handle.Result);
    }
    private IEnumerator LoadEnemyStatsList() {
        string address = "EnemyStatData";
        AsyncOperationHandle<EnemyStatData> handle = Addressables.LoadAssetAsync<EnemyStatData>(address);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
            EnemyStateSystem.SetEnemyStatesTemplateDtny(handle.Result);
    }

}
