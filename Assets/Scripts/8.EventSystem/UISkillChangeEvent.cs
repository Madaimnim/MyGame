public class UISkillChangeEvent
{
    public int slotIndex{get;private set;}
    public PlayerStateManager.PlayerStats.SkillData newSkill { get; private set; }

    public UISkillChangeEvent(int slotIndex, PlayerStateManager.PlayerStats.SkillData newSkill) {
        this.slotIndex = slotIndex;
        this.newSkill = newSkill;
    }

}
