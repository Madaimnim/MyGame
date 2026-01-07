using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Stage/StageDataTable")]
public class StageDataTable : ScriptableObject {

    public List<StageData> Stages;
    public StageData GetStage(int stageId) {
        return Stages.Find(s => s.StageId == stageId);
    }
}
