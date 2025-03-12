public class EquipSkillEvent
{
    public int slotIndex { get; private set; }
    public int skillID { get; private set; }

    public EquipSkillEvent(int slotIndex, int skillID) {
        this.slotIndex = slotIndex;
        this.skillID = skillID;
    }
}
