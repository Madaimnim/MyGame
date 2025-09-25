using UnityEngine;

public class UI_BattlePlayerPanel : MonoBehaviour
{
    [Header("�lUI")]
    public UI_HpSlider hpSlider;
    public Transform skillUIParent; // ��ޯ�N�o UI �� Panel
    public UI_SkillCooldownPanelController skillCooldownPanelController; // �s�W

    private Player boundPlayer;

    //UIManager_Player���U�AUIManager_SkillCooldown���U
    #region Setup(Player player)
    public void Setup(Player player) {
        boundPlayer = player;

        // �j�w���
        if (hpSlider != null)
        {
            hpSlider.title = boundPlayer.Runtime.StatsData.Name;
            hpSlider.Bind(player);
        }

        // �j�w�ޯ�N�o UI
        if (skillCooldownPanelController != null)
        {
            skillCooldownPanelController.RegisterPlayerSkillSliders(player);
        }
    }
    #endregion

    //��UIManager�T�{�w���U��ơA���P�ϥ�
    #region IsBoundTo(Player player)
    public bool IsBoundTo(Player player) => boundPlayer == player;
    #endregion
}
