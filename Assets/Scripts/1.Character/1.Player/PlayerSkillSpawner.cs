using System.Collections;
using UnityEngine;


public class PlayerSkillSpawner : MonoBehaviour
{
    private Player player;

    private void Awake() {
        player = GetComponent<Player>();
    }

    //生成技能
    public void SpawnSkill(int slotIndex, PlayerSkillRuntime playerSkillRuntime, GameObject detector) {
        TargetDetector targetDetector = detector.GetComponent<TargetDetector>();
        if (targetDetector == null || !targetDetector.hasTarget || targetDetector.targetTransform == null)
        {
            Debug.Log(" 沒有有效目標，取消技能生成");
            return;
        }

        // 攻擊力計算
        int playerAttackPower = player.GetPlayerAttackPower();
        int skillAttackPower = playerSkillRuntime.SkillPower;
        int finalAttackPower = playerAttackPower + skillAttackPower;

        // 計算方向與旋轉角度
        Vector2 directionVector = (Vector2)(targetDetector.targetTransform.position - transform.position).normalized;
        float rotateAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;

        // 生成技能物件，設定屬性
        GameObject currentSkillPrefab = Instantiate(playerSkillRuntime.SkillPrefab, transform.position, Quaternion.identity);
        currentSkillPrefab.SetActive(false); // 先關閉避免出現異常
        SetSkillObjectProperties(currentSkillPrefab, directionVector, finalAttackPower, targetDetector.targetTransform, rotateAngle);
        currentSkillPrefab.SetActive(true);

        player.Runtime.EquippedSkillSlots[slotIndex].TriggerCooldown(playerSkillRuntime.SkillCooldown);

        player.Runtime.OnSkillUsed(slotIndex, transform);

    }

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
}
