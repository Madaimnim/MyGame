using UnityEngine;

public class UIController_Player : MonoBehaviour
{
    [Header("�lUI")]
    public UIController_HpSlider hpUI;
    public Transform skillUIParent; // ��ޯ�N�o UI �� Panel

    private Player boundPlayer;

    //��UIManager_Player���U�ɨϥΡA�P�ɶi��UIManager_SkillCooldown���U
    #region Setup(Player player)
    public void Setup(Player player) {
        boundPlayer = player;


        // �j�w���
        if (hpUI != null)
        {
            hpUI.title = player.playerStats.playerName;
            hpUI.Bind(player);
        }

        // ���U�ޯ�N�o UI
        if (UIManager_SkillCooldown.Instance != null)
        {
            UIManager_SkillCooldown.Instance.RegisterPlayerSkills(player, skillUIParent);
        }
    }
    #endregion

    //��UIManager�T�{�w���U��ơA���P�ϥ�
    #region IsBoundTo(Player player)
    public bool IsBoundTo(Player player) => boundPlayer == player;
    #endregion
}
