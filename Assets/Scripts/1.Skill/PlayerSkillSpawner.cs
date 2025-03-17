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
    public void SpawnAttack(GameObject skillPrefab, Transform targetTransform) {
        if (skillPrefab == null || targetTransform == null)
        {
            Debug.LogError("❌ SpawnAttack: SkillPrefab 或 TargetTransform 為空！");
            return;
        }

        // 計算方向
        Vector2 directionVector = (Vector2)(targetTransform.position - transform.position).normalized;
        float rotateAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

        // 生成技能
        GameObject currentSkillPrefab = Instantiate(skillPrefab, transform.position, Quaternion.identity);
        currentSkillPrefab.SetActive(false);               // 避免異常行為，先關閉

        // 設置技能屬性
        SetSkillObjectProperties(currentSkillPrefab, directionVector, player.playerStats.attackPower, targetTransform, rotateAngle);
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
