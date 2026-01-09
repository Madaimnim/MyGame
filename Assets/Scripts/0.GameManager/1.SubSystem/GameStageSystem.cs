using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStageSystem : GameSubSystem {
    //public PlayerSpawnController PlayerSpawnController { get; private set; }

    public StageTable StageTable { get; private set; }
    public StageData CurrentStageData { get; private set; }
    public GameStageSystem(GameManager gm) : base(gm) { }
    public override void Initialize() {}

    public void RequestEnterStage(int stageId) {
        var stageData = StageTable.GetStageData(stageId);
        if (stageData == null) {
            Debug.LogError($"Stage:{stageId} not found");
            return;
        }
        CurrentStageData = stageData;

        //負責告訴 State：要進 Battle 了
        GameManager.Instance.GameStateSystem.SetState(GameState.Battle);
    }

    public void SetStageDataDtny(StageTable stageTable) {
        StageTable = stageTable;
    }
}
