using UnityEngine;
using System.Collections.Generic;

public class PlayerSkillSpawner : MonoBehaviour
{
    #region 公開變數
    public Player player;
    //Todo
    public Transform targetTransform;
    #endregion

    #region 私有變數
    private GameObject currentSkillPrefab;
    #endregion

    #region AnimationEvent方法()
    public void SpawnAttack() {
        int skillSlotIndex = 0;

        int currentSkillID = player.playerStats.equippedSkillIDList[skillSlotIndex];
        if (!player.playerStats.skillPoolDtny.TryGetValue(currentSkillID, out var currentSkill))
        {
            Debug.LogError($"❌ AttackSpawner: 找不到技能 ID {currentSkillID}！");
            return;
        }
        if (currentSkill.skillPrefab == null)
        {
            Debug.LogWarning("❌ AttackSpawner: skillPrefab 未設定！");
            return;
        }

        //目標方向
        Vector2 directionVector = targetTransform != null
            ? (Vector2)(targetTransform.position - transform.position).normalized
            : Vector2.right; // 默認向右

        //旋轉角度
        float rotateAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

        currentSkillPrefab = Instantiate(currentSkill.skillPrefab, transform.position, Quaternion.Euler(0, 0, rotateAngle));
        currentSkillPrefab.SetActive(false);               // 避免異常行為，先關閉

        //默認移動方向(1,0)
        SetSkillObjectProperties(
            currentSkillPrefab, 
            new Vector2(1, 0), 
            player.playerStats.attackPower, 
            targetTransform, 
            rotateAngle);

        currentSkillPrefab.SetActive(true); // 啟用
    }
    #endregion

    #region 設定技能屬性
    private void SetSkillObjectProperties(GameObject currentSkillPrefab, Vector2 moveDirection,int baseAttack,Transform targetTransform, float rotateAngle) {
        SkillObject skillObject = currentSkillPrefab.GetComponent<SkillObject>();
        if (skillObject != null)
        {
            skillObject.SetSkillProperties(
                moveDirection,
                baseAttack,
                targetTransform,
                rotateAngle
            );
        }
        else
        {
            Debug.LogError("❌ AttackSpawner: 無法找到 Player001Skill001 腳本！");
        }
    }
    #endregion
}
