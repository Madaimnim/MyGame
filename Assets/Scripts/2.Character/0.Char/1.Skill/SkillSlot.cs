using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSlot
{
    //Todo 實現怪物的技能槽安裝機制，下一步評估技能偵測器物件化
    //SkillDetectorBase 下轄Circle_Detector、Box_Detector等偵測策略，可生成範圍Sprite物件，無實際技能槽，開關僅關閉可視化範圍

    /// <summary>玩家的技能槽目前只有PlayerStateSystmem透過PlayerSkillSystem，在解鎖腳色時安裝;另外也可透過UIController進行技能安裝如下:
    /// 
    /// UnlockPlayer(int id)->
    /// PlayerSkillSystem.EquipPlayerSkill(int playerId, int slotIndex, int skillId)->
    /// SkillComponent.EquipSkill(slotIndex, skillId)->
    /// SkillSlot.SetSlot(int skillId,SkillDetectorBase detectStrategy = null)
    /// 
    /// </summary>

    public int SkillId { get; private set; } = -1;
    public float CooldownTimer { get; private set; }
    public SkillDetectorBase Detector { get; private set; }
    public GameObject RangeObject;

    public bool HasSkill => SkillId  != -1;   //-1 代表無技能;
    public bool IsReady => SkillId != -1 && CooldownTimer <= 0f;
    
    private Transform _owner;
    private IReadOnlyList<IInteractable> _targetList;

    public SkillSlot(Transform owner, IReadOnlyList<IInteractable> targetList) {
        SkillId = -1;
        CooldownTimer = 0f;
        Detector = null;

        _owner = owner;
        _targetList= targetList;
    }

    public void Tick() {
        if(Detector != null) Detector.DetectTargetsTick(_targetList);
        TickCooldown();
    }

    public void SetSlot(int skillId,SkillDetectorBase detectStrategy = null) {
        SkillId = skillId;
        Detector = detectStrategy;
        CooldownTimer = 0f;
        Detector.Initialize(_owner);

        if (RangeObject != null) GameObject.Destroy(RangeObject);
        RangeObject = Detector.SpawnRangeObject(_owner);
        RangeObject.transform.SetParent(_owner);
        RangeObject.transform.localPosition = Vector3.zero;
    }
    public void TriggerCooldown(float cd) {
        if (SkillId == -1) return;
        CooldownTimer = cd;
    }

    private void TickCooldown()=>CooldownTimer = Mathf.Max(0, CooldownTimer - Time.deltaTime);  

    public void Uninstall() {
        SkillId = -1;
        CooldownTimer = 0f;
        Detector = null;
    }

}