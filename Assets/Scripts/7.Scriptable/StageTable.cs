using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Stage/StageTable")]
public class StageTable : ScriptableObject {
    public List<StageData> Stages;
    public StageData GetStageData(int stageId) {
        return Stages.Find(stageData => stageData.StageId == stageId);
    }
}
