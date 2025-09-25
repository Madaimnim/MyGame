using UnityEngine;

public class UI_BattlePlayerPanel : MonoBehaviour
{
    [Header("子UI")]
    public UI_HpSlider hpSlider;
    public Transform skillUIParent; // 放技能冷卻 UI 的 Panel
    public UI_SkillCooldownPanelController skillCooldownPanelController; // 新增

    private Player boundPlayer;

    //UIManager_Player註冊，UIManager_SkillCooldown註冊
    #region Setup(Player player)
    public void Setup(Player player) {
        boundPlayer = player;

        // 綁定血條
        if (hpSlider != null)
        {
            hpSlider.title = boundPlayer.Runtime.StatsData.Name;
            hpSlider.Bind(player);
        }

        // 綁定技能冷卻 UI
        if (skillCooldownPanelController != null)
        {
            skillCooldownPanelController.RegisterPlayerSkillSliders(player);
        }
    }
    #endregion

    //供UIManager確認已註冊資料，註銷使用
    #region IsBoundTo(Player player)
    public bool IsBoundTo(Player player) => boundPlayer == player;
    #endregion
}
