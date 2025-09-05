using UnityEngine;

public class PlayerExpUp : MonoBehaviour
{
    public Player player;
    private PlayerStateManager.PlayerStatsRuntime Stats => player.playerStats;

    void Awake() { }
    void OnEnable() {
        ExpManager.Instance?.Register(this);
    }

    void OnDisable() {
        ExpManager.Instance?.Unregister(this);
    }


    // ���o�U�@�ũһݸg��
    public int GetExpToNextLevel() {
        if (Stats.level >= Stats.expTable.Length)
            return int.MaxValue;

        return Stats.expTable[Stats.level - 1];
    }

    // �W�[�g��ȡA���ˬd�O�_�ɯ�(while�i�H���ƽT�{�O�_�s��ɯ�)
    public void AddExp(int amount) {
        if (amount <= 0) return;

        Debug.Log($"�o�Ӹ}��u{gameObject.name}�v���o�g���{amount}");
        Stats.currentEXP += amount;

        while (Stats.currentEXP >= GetExpToNextLevel() && Stats.level < Stats.expTable.Length)
        {
            Stats.currentEXP -= GetExpToNextLevel();
            Stats.level++;
            LevelUp();
        }
    }

    // �ɯŮɷ|���檺�Ť�k
    private void LevelUp() {
        Debug.Log($"�o�Ӹ}��u{gameObject.name}�v�ͨ�F����{Stats.level}");
        TextPopupManager.Instance.ShowLevelUpPopup(Stats.level, transform);
        
        //�Ȱ��C��
        //GamePauseManager.Instance.PauseGame();
    }
}