using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillSpawner : MonoBehaviour
{
    #region 公開變數
    public Player player;
    public float InstantiatOffsetX = 0.0f;
    public float InstantiatOffsetY = 0.0f;
    #endregion

    #region 私有變數
    private GameObject currentAttackObject;
    #endregion

    #region AnimationEvent方法()
    public void SpawnAttack() {
        int skillSlotIndex = 0;

        int currentSkillID = player.playerStats.equippedSkillIDList[skillSlotIndex];

        //  修正為 Dictionary 讀取技能
        if (!player.playerStats.skillPoolDtny.TryGetValue(currentSkillID, out var currentSkill))
        {
            Debug.LogError($"❌ AttackSpawner: 找不到技能 ID {currentSkillID}！");
            return;
        }

        //  確保當前等級數據存在
        if (!currentSkill.skillLevelsDataDtny.TryGetValue(currentSkill.currentLevel, out var skillLevelData))
        {
            Debug.LogError($"❌ AttackSpawner: 技能 {currentSkill.skillName} 缺少等級 {currentSkill.currentLevel} 的數據！");
            return;
        }

        //  確保技能有攻擊 Prefab
        if (skillLevelData.attackPrefab == null)
        {
            Debug.LogWarning("❌ AttackSpawner: attackPrefab 未設定！");
            return;
        }

        //  計算生成位置
        Vector2 spawnPosition = new Vector2(transform.position.x + InstantiatOffsetX, transform.position.y + InstantiatOffsetY);

        //  生成攻擊物件
        currentAttackObject = Instantiate(skillLevelData.attackPrefab, spawnPosition, Quaternion.identity);
        currentAttackObject.SetActive(false); // 避免異常行為，先關閉

        //  設定攻擊物件屬性
        SetSkillObject(currentAttackObject, new Vector2(1, 0), skillLevelData);

        currentAttackObject.SetActive(true); // 啟用
    }
    #endregion

    #region 設定技能屬性
    private void SetSkillObject(GameObject attackObject, Vector2 moveDirection, PlayerStateManager.PlayerStats.SkillLevelData skillData) {
        SkillObject skillScript = attackObject.GetComponent<SkillObject>();

        if (skillScript != null)
        {

            skillScript.SetSkillProperties(
                moveDirection,
                skillData.moveSpeed,
                skillData.knockForce,
                skillData.destroySelfDelay,
                skillData.attackPower,
                skillData.targetLayers
            );
        }
        else
        {
            Debug.LogError("❌ AttackSpawner: 無法找到 Player001Skill001 腳本！");
        }
    }
    #endregion
}
