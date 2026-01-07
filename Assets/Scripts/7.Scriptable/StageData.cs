using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageWave {
    public int EnemyId;
    public int SpawnCount;
}

[CreateAssetMenu(menuName = "Stage/StageData")]
public class StageData : ScriptableObject {
    
    public int StageId;

    public float SpawnInterval = 1f;
    public List<StageWave> Waves;
}
