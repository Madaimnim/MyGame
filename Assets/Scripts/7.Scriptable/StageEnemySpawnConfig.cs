using UnityEngine;

[CreateAssetMenu(fileName = "StageEnemySpawnConfig",menuName = "GameData/StageEnemySpawnConfig")]
public class StageEnemySpawnConfig : ScriptableObject {
    [Header("生成間隔")]
    public float SpawnInterval = 0.1f;

    [System.Serializable]
    public class SpawnWave {
        public int EnemyId;
        public int SpawnCount;
    }

    [Header("生成波次")]
    public SpawnWave[] Waves;
}
