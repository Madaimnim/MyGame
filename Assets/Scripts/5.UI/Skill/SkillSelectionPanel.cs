using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSelectionPanel : MonoBehaviour
{
 //   #region 🔹 公開變數
 //   public SkillController skillController;
 //   public GameObject skillButtonPrefab;
 //   public Transform skillButtonContainer;
 //   public GameObject panel;
 //   public float xOffset = 50f;
 //   public float yOffset = -50f;
 //   #endregion
 //
 //   #region 🔹 私有變數
 //   private int currentSlotIndex;
 //   private int playerID;
 //   #endregion
 //
 //   #region 🔹 初始化
 //   private void Start() {
 //       HidePanel();
 //
 //       if (skillController == null)
 //       {
 //           Debug.LogError("❌ [SkillSelectionPanel] SkillController 未設定！");
 //           return;
 //       }
 //
 //       playerID = skillController.player.playerID;
 //   }
 //   #endregion
 //
 //   #region 🔹 顯示技能選擇面板
 //   public void ShowSelectionPanel(int slotIndex, Transform skillSlotButton) {
 //       if (panel == null)
 //       {
 //           Debug.LogError("❌ [SkillSelectionPanel] panel 未設定！");
 //           return;
 //       }
 //
 //       currentSlotIndex = slotIndex;
 //       panel.SetActive(true);
 //
 //       // **對齊技能槽按鈕**
 //       if (skillSlotButton != null)
 //       {
 //           RectTransform panelRect = panel.GetComponent<RectTransform>();
 //           RectTransform slotButtonRect = skillSlotButton.GetComponent<RectTransform>();
 //
 //           Vector3 worldPosition = slotButtonRect.position;
 //           panelRect.position = new Vector3(worldPosition.x + xOffset, worldPosition.y + yOffset, worldPosition.z);
 //       }
 //       else
 //       {
 //           Debug.LogWarning($"⚠️ [SkillSelectionPanel] 無法獲取技能槽 {slotIndex} 的按鈕！");
 //       }
 //
 //       // **清空現有按鈕**
 //       foreach (Transform child in skillButtonContainer)
 //       {
 //           Destroy(child.gameObject);
 //       }
 //
 //       // **獲取該角色擁有的技能**
 //       var playerState = PlayerStateManager.Instance.GetPlayerState(playerID);
 //       if (playerState == null)
 //       {
 //           Debug.LogError($"❌ [SkillSelectionPanel] 找不到玩家 ID {playerID} 的數據！");
 //           return;
 //       }
 //
 //       List<int> ownedSkillIDs = playerState.ownedSkills;
 //       HashSet<int> equippedSkillIDs = new HashSet<int>(playerState.equippedSkills);
 //
 //       // **顯示可選擇的技能（排除已裝備的）**
 //       foreach (int skillID in ownedSkillIDs)
 //       {
 //           if (equippedSkillIDs.Contains(skillID)) continue;
 //
 //           PlayerSkillData.PlayerSkillStats skillStats = PlayerStateManager.Instance.GetPlayerSkill(skillID);
 //           if (skillStats == null) continue;
 //
 //           GameObject newButton = Instantiate(skillButtonPrefab, skillButtonContainer);
 //           TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
 //
 //           if (buttonText == null)
 //           {
 //               Debug.LogError("❌ [SkillSelectionPanel] skillButtonPrefab 內部缺少 TMP_Text 組件！");
 //               return;
 //           }
 //
 //           buttonText.text = skillStats.skillName;
 //           int selectedSkillID = skillStats.skillID;
 //           newButton.GetComponent<Button>().onClick.AddListener(() => SelectSkill(selectedSkillID));
 //       }
 //   }
 //   #endregion
 //
 //   #region 🔹 選擇技能
 //   public void SelectSkill(int skillID) {
 //       if (skillController != null)
 //       {
 //           skillController.SetSkillSlot(currentSlotIndex, skillID);
 //           HidePanel();
 //       }
 //       else
 //       {
 //           Debug.LogError("❌ [SkillSelectionPanel] skillController 未設定！");
 //       }
 //   }
 //   #endregion
 //
 //   #region 🔹 隱藏面板
 //   public void HidePanel() {
 //       panel.SetActive(false);
 //   }
 //   #endregion
}
