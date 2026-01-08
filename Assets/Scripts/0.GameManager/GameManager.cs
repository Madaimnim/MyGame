using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

[DefaultExecutionOrder(-50)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform EnemyBattleParent;
    public LineRenderer LineRenderer;
    public Transform PlayerBattleParent;
    private readonly List<GameSubSystem> _subSystems = new();

    [SerializeField] private PrefabConfig _prefabConfig;
    [SerializeField] private SceneConfig _sceneConfig;

    //事件
    public event Action OnAllDataLoaded;
    public event Action OnAllSubSystemReady;

    //子系統
    public PlayerStateSystem PlayerStateSystem { get; private set; }
    public EnemyStateSystem EnemyStateSystem { get; private set; }

    public GameStageSystem GameStageSystem { get; private set; }
    public GameStateSystem GameStateSystem { get; private set; }
    private GameStateRouter _gameStateRouter;
    public Dictionary<GameState, IGameStateHandler> GameStateHandlers { get; private set; }
    public GameSceneSystem GameSceneSystem { get; private set; }


    public bool IsAllDataLoaded { get; private set; } = false;

    //Config
    public PrefabConfig PrefabConfig => _prefabConfig;
    public SceneConfig SceneConfig => _sceneConfig;

    //繼承GameSubSystem的子系統，建構子時自動訂閱
    public void RegisterSubsystem(GameSubSystem subSystem)
    {
        if (!_subSystems.Contains(subSystem))
            _subSystems.Add(subSystem);
    }

    private void Awake()
    {
        //單例
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //建子系統
        GameStageSystem = new GameStageSystem(this);
        GameStateSystem = new GameStateSystem(this);
        GameSceneSystem = new GameSceneSystem(this);
        PlayerStateSystem = new PlayerStateSystem(this);
        EnemyStateSystem = new EnemyStateSystem(this);
        _gameStateRouter = new GameStateRouter(this);

        //建 Handler map並建構子給_gameStateRouter
        var runner = new CoroutineRunnerAdapter(this);
        GameStateHandlers = new Dictionary<GameState, IGameStateHandler>{
            { GameState.GameStart, new GameStartHandler(runner,GameSceneSystem, PlayerStateSystem) },
            { GameState.Preparation, new PreparationHandler( GameSceneSystem) },
            { GameState.Battle, new BattleHandler( GameSceneSystem, PlayerStateSystem) },
        };


        //所有子系統初始化
        foreach (var sub in _subSystems) sub.Initialize();

        //發事件
        OnAllSubSystemReady?.Invoke();

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
    }

    private void OnDisable()
    { }

    private void OnEnable()
    { }

    private IEnumerator Start()
    {
        yield return Addressables.InitializeAsync();
        yield return LoadAssetAsync<PlayerStatData>("PlayerStatData",playerStatData => PlayerStateSystem.SetPlayerStatsRuntimeDtny(playerStatData));
        yield return LoadAssetAsync<EnemyStatData>("EnemyStatData", enemyStatData => EnemyStateSystem.SetEnemyStatsTemplateDtny(enemyStatData));
        yield return LoadAssetAsync<StageTable>("StageTable",stageTable=> GameStageSystem.SetStageDataDtny(stageTable));

        IsAllDataLoaded = true;
        OnAllDataLoaded?.Invoke();

        // 遊戲一開始 → 自動切到 StartScene
        GameSceneSystem.LoadSceneByKey("Start");
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        foreach (var sub in _subSystems) sub.Update(dt);
    }

    //外部有提供一個Action方法引用，並用這次載入完成的結果當參數，呼叫那個方法
    private IEnumerator LoadAssetAsync<T>(string address,Action<T> onLoaded) {
        var handle = Addressables.LoadAssetAsync<T>(address);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded) 
            onLoaded(handle.Result);
        else 
            Debug.LogError($"LoadAssetAsync<{typeof(T).Name}> failed : {address}");
        
    }

}