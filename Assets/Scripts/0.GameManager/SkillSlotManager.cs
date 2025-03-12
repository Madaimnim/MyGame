using UnityEngine;

public class SkillSlotManager : MonoBehaviour
{
    private PlayerStateManager.PlayerStats currentPlayer;

    private void OnEnable() {
        EventBus.Listen<UICurrentPlayerChangEvent>(OnCurrentPlayerChanged);
        EventBus.Listen<EquipSkillEvent>(OnEquipSkill);
    }

    private void OnDisable() {
        EventBus.StopListen<UICurrentPlayerChangEvent>(OnCurrentPlayerChanged);
        EventBus.StopListen<EquipSkillEvent>(OnEquipSkill);
    }

    private void OnCurrentPlayerChanged(UICurrentPlayerChangEvent eventData) {
        currentPlayer = eventData.currentPlayer;
        Debug.Log($"🟢 [SkillSlotManager] 角色變更: {currentPlayer.playerName} (ID: {currentPlayer.playerID})");
        LoadEquippedSkills();
    }


    private void LoadEquippedSkills() {
        if (currentPlayer == null)
        {
            Debug.LogError("❌ [SkillSlotManager] currentPlayer 為 null，無法加載技能！");
            return;
        }

        for (int i = 0; i < currentPlayer.equippedSkillIDList.Count; i++)
        {
            int skillID = currentPlayer.equippedSkillIDList[i];
            var skill = currentPlayer.GetSkillAtSkillSlot(i);
            EventBus.Trigger(new UISkillChangeEvent(i, skill));
        }
    }

    private void OnEquipSkill(EquipSkillEvent eventData) {
        if (currentPlayer == null) return;

        currentPlayer.SetSkillAtSlot(eventData.slotIndex, eventData.skillID);
        var newSkill = currentPlayer.GetSkillAtSkillSlot(eventData.slotIndex);

        EventBus.Trigger(new UISkillChangeEvent(eventData.slotIndex, newSkill));
    }
}
