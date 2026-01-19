public interface ISkillUseGate {
    bool CanUse(ISkillRuntime skill, int inputSlotNumber);
    void Consume(ISkillRuntime skill, int inputSlotNumber);//代價、資源扣除
}
