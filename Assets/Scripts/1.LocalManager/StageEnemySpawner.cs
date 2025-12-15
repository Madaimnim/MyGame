using System.Collections;
using UnityEngine;

public class StageEnemySpawner : MonoBehaviour {

    [Header("生成條件設定")]
    public float SpawnInterval = 1.5f;
    [Header("生成區域（BoxCollider2D）")]
    public BoxCollider2D SpawnArea;

    [System.Serializable]
    public class SpawnWave {
        public int EnemyId;
        public int SpawnCount;
    }
    [Header("生成波次設定")]
    public SpawnWave[] Waves;


    private void Start() {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine() {
        for (int w = 0; w < Waves.Length; w++) {
            SpawnWave wave = Waves[w];

            for (int i = 0; i < wave.SpawnCount; i++) {
                Vector3 spawnPos = GetRandomPositionInArea();

                GameManager.Instance.EnemyStateSystem
                    .SpawnSystem
                    .SpawnEnemy(wave.EnemyId, spawnPos);

                yield return new WaitForSeconds(SpawnInterval);
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
