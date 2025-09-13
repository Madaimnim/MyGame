using UnityEngine;

public class UIController_Player : MonoBehaviour
{
    [Header("子UI")]
    public UIController_HpSlider hpUI;
    public Transform skillUIParent; // 放技能冷卻 UI 的 Panel

    private Player boundPlayer;

    //供UIManager_Player註冊時使用，同時進行UIManager_SkillCooldown註冊
    #region Setup(Player player)
    public void Setup(Player player) {
        boundPlayer = player;


        // 綁定血條
        if (hpUI != null)
        {
            hpUI.title = player.playerStats.playerName;
            hpUI.Bind(player);
        }

        // 註冊技能冷卻 UI
        if (UIManager_SkillCooldown.Instance != null)
        {
            UIManager_SkillCooldown.Instance.RegisterPlayerSkills(player, skillUIParent);
        }
    }
    #endregion

    //供UIManager確認已註冊資料，註銷使用
    #region IsBoundTo(Player player)
    public bool IsBoundTo(Player player) => boundPlayer == player;
    #endregion
}
