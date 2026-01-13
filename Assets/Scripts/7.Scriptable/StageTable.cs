using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(menuName = "Stage/StageTable")]
public class StageTable : ScriptableObject {
    [FormerlySerializedAs("StageDataList")]
    public List<StageData> StageDataList;
    public StageData GetStageData(int stageId) {
        return StageDataList.Find(stageData => stageData.StageId == stageId);
    }

    public StageData GetNextStage(int stageId) {
        int index = StageDataList.FindIndex(s => s.StageId == stageId);
        if (index < 0 || index + 1 >= StageDataList.Count) return null;

        return StageDataList[index + 1];
    }

}
