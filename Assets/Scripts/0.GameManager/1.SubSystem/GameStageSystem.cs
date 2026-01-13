using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStageSystem : GameSubSystem {
    //public PlayerSpawnController PlayerSpawnController { get; private set; }

    public StageTable StageTable { get; private set; }
    public StageData CurrentStageData { get; private set; }
    public GameStageSystem(GameManager gm) : base(gm) { }
    public bool IsBattleStarted { get; private set; }
    public bool IsBattleEnded { get; private set; }
    private HashSet<int> _unlockedStageHashSet = new();

    public override void Initialize() {
        GameEventSystem.Instance.Event_BattleStart += () => SetIsBattleStarted(true);
        GameEventSystem.Instance.Event_OnWallBroken += () => SetIsBattleEneded(true);
        UnlockStage(101); //預設解鎖第一關
    }

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



    public bool IsStageUnlocked(int stageId) {
        return _unlockedStageHashSet.Contains(stageId);
    }

    public void UnlockStage(int stageId) {
        _unlockedStageHashSet.Add(stageId);
    }

    public void MarkStageCleared(int stageId) {
        StageData nextStageData = StageTable.GetNextStage(stageId);
        if (nextStageData != null) UnlockStage(nextStageData.StageId);
    }



    public void SetStageDataDtny(StageTable stageTable) {
        StageTable = stageTable;
    }
    public void ResetBattleState() {
        IsBattleStarted = false;
        IsBattleEnded = false;
    }
    public void SetIsBattleStarted(bool value) => IsBattleStarted = value;
    public void SetIsBattleEneded(bool value) => IsBattleEnded = value;
}
