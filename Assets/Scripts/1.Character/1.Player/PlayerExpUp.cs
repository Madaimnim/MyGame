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


    // 取得下一級所需經驗
    public int GetExpToNextLevel() {
        if (Stats.level >= Stats.expTable.Length)
            return int.MaxValue;

        return Stats.expTable[Stats.level - 1];
    }

    // 增加經驗值，並檢查是否升級(while可以重複確認是否連續升級)
    public void AddExp(int amount) {
        if (amount <= 0) return;

        Debug.Log($"這個腳色「{gameObject.name}」取得經驗值{amount}");
        Stats.currentEXP += amount;

        while (Stats.currentEXP >= GetExpToNextLevel() && Stats.level < Stats.expTable.Length)
        {
            Stats.currentEXP -= GetExpToNextLevel();
            Stats.level++;
            LevelUp();
        }
    }

    // 升級時會執行的空方法
    private void LevelUp() {
        Debug.Log($"這個腳色「{gameObject.name}」生到了等級{Stats.level}");
        TextPopupManager.Instance.ShowLevelUpPopup(Stats.level, transform);
        
        //暫停遊戲
        //GamePauseManager.Instance.PauseGame();
    }
}