public class UISkillChangeEvent
{
    public int slotIndex{get;private set;}
    public PlayerStateManager.PlayerStatsRuntime.SkillData newSkill { get; private set; }

    public UISkillChangeEvent(int slotIndex, PlayerStateManager.PlayerStatsRuntime.SkillData newSkill) {
        this.slotIndex = slotIndex;
        this.newSkill = newSkill;
    }

}
