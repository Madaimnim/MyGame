public class UISkillChangeEvent
{
    public int slotIndex{get;private set;}
    public PlayerSkillRuntime newSkill { get; private set; }

    public UISkillChangeEvent(int slotIndex, PlayerSkillRuntime newSkill) {
        this.slotIndex = slotIndex;
        this.newSkill = newSkill;
    }

}
