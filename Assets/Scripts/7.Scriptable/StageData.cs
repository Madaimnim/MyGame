using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageWave {
    public int EnemyId;
    public int SpawnCount;
}


[CreateAssetMenu(menuName = "Stage/StageData")]
public class StageData : ScriptableObject {
    
    public int StageId;             //101,102,103
    public string SceneKey => $"Battle{StageId}";         //Battle101,Battle102,Battle103
    public string StageName;        //1,2,3,4,5

    public float SpawnInterval = 1f;
    public List<StageWave> Waves;
}
