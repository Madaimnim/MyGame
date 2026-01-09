using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;

//生成的敵人將訂閱Counter的計數
public class EnemySpawner : MonoBehaviour {
    
    [Header("敵人生成位置")]
    [SerializeField] private MonoBehaviour _enemyPositionProvider;
    private IPositionProvider PositionProvider => _enemyPositionProvider as IPositionProvider;

    private StageData _stageData;
    private DefaultEnemyFactory _enemyFactory;
    private EnemyCounter _enemycounter;

    private void Awake() {
        _enemycounter=GetComponent<EnemyCounter>();
    }
    private void OnEnable() {
        _enemyFactory = new DefaultEnemyFactory();
        if (GameManager.Instance.GameStageSystem != null) _stageData = GameManager.Instance.GameStageSystem.CurrentStageData;

    }
    private void Start() {}

    public void SpawnBegin() {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine() {
        foreach (var wave in _stageData.Waves) {
            for (int i = 0; i < wave.SpawnCount; i++) {
                Vector3 spawnPos = PositionProvider.GetPosition();

                var enemy = _enemyFactory.CreateEnemy(wave.EnemyId);
                var appearEffector = AppearEffectorFactory.CreateEffector(_stageData.EnemyAppearType);
                StartCoroutine(appearEffector.Play(enemy.gameObject, spawnPos));

                enemy.HealthComponent.OnDie += () => _enemycounter.EnemyDefeated(enemy.Rt.Exp);

                yield return new WaitForSeconds(_stageData.SpawnInterval);
            }
        }
    }
}
