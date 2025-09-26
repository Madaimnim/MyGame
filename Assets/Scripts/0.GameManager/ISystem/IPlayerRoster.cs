

public interface IPlayerRoster 
{
    void Unlock(int id);
    void SpawnBoth(int id);
    PlayerStatsRuntime GetState(int id);
    void SetupSkillSlot(int id, int slot, int skillId);
    void ActivateAll();
    void DeactivateAll();
}
