public interface ISkillUseGate {
    bool CanUse(ISkillRuntime skill, int slotIndex);
    void Consume(ISkillRuntime skill, int slotIndex);//代價、資源扣除
}
