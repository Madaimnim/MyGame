using System.Collections;
using UnityEngine;

public class StageEnemySpawner : MonoBehaviour {
    
    [Header("生成區域（BoxCollider2D）")]
    public BoxCollider2D SpawnArea;
    [Header("怪物生成Config檔")]
    private StageData _stageData;
    private void OnEnable() {
        if(GameManager.Instance.GameStageSystem != null) 
            _stageData = GameManager.Instance.GameStageSystem.CurrentStageData;
    }
    private void Start() {
        if (_stageData == null) {
            Debug.LogError("StageEnemySpawner 缺少 StageData");
            return;
        }
        StartCoroutine(SpawnCoroutine());
    }
    private IEnumerator SpawnCoroutine() {
        foreach (var wave in _stageData.Waves) {
            for (int i = 0; i < wave.SpawnCount; i++) {
                Vector3 spawnPos = GetRandomPositionInArea();

                GameManager.Instance.EnemyStateSystem
                    .SpawnSystem
                    .SpawnEnemy(wave.EnemyId, spawnPos);

                yield return new WaitForSeconds(_stageData.SpawnInterval);
            }
        }
    }
    private Vector3 GetRandomPositionInArea() {
        Bounds bounds = SpawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector3(x, y, 0f);
    }
}
