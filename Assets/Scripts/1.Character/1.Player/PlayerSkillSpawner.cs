using System.Collections;
using UnityEngine;


public class PlayerSkillSpawner : MonoBehaviour
{
    private Player player;

    private void Awake() {
        player = GetComponent<Player>();
    }

    //延遲生成技能
    #region SpawnSkillAfterDelay(float delayTime, GameObject skillPrefab, GameObject detectPrefab)
    public IEnumerator SpawnSkillAfterDelay(int slotIndex, float delayTime, GameObject skillPrefab, GameObject detectPrefab) {
        yield return new WaitForSeconds(delayTime);

        if (detectPrefab != null)
        {
            TargetDetector detector = detectPrefab.GetComponent<TargetDetector>();
            if (detector != null && detector.targetTransform != null)
            {
                SpawnAttack(player.playerStats.attackPower, skillPrefab, detector.targetTransform, player.playerStats.GetSkillAtSkillSlot(slotIndex).attack);

                int currentSkillUsageCount = player.playerStats.GetSkillAtSkillSlot(slotIndex).skillUsageCount++;
                int nextSkillUsageCount = player.playerStats.GetSkillAtSkillSlot(slotIndex).nextSkillLevelCount;
                if (currentSkillUsageCount >= nextSkillUsageCount)
                    player.playerAI.SkillLevelUp(slotIndex);
            }
        }
    }

    public void SpawnAttack(int playerAttack,GameObject skillPrefab, Transform targetTransform,int skillAttack) {
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
        SetSkillObjectProperties(currentSkillPrefab, directionVector, playerAttack+skillAttack, targetTransform, rotateAngle);
        currentSkillPrefab.SetActive(true); // 啟用
    }
    #endregion

    #region 設定技能屬性
    private void SetSkillObjectProperties(GameObject currentSkillPrefab, Vector2 directionVector, int finalAttack,Transform targetTransform, float rotateAngle) {
        SkillObject skillObject = currentSkillPrefab.GetComponent<SkillObject>();
        if (skillObject != null)
        {
            skillObject.SetSkillProperties(
                directionVector,
                finalAttack,
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
