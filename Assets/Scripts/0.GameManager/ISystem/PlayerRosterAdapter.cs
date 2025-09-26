public class PlayerRosterAdapter : IPlayerRoster
{
    public void Unlock(int id) => PlayerStateManager.Instance.UnlockPlayer(id);
    public void SpawnBoth(int id) => PlayerStateManager.Instance.SpawnBothPlayers(id);
    public PlayerStatsRuntime GetState(int id) => PlayerStateManager.Instance.GetState(id);
    public void SetupSkillSlot(int id, int slot, int skillId)
        => PlayerStateManager.Instance.SetupPlayerSkillSlot(id, slot, skillId);
    public void ActivateAll() => PlayerStateManager.Instance.ActivateAllPlayer();
    public void DeactivateAll() => PlayerStateManager.Instance.DeactivateAllPlayer();
}