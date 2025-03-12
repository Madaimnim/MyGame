using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillPanel : MonoBehaviour
{
    //#region 公開變數
    //public SkillController skillController;
    //public SkillSelectionPanel skillSelectionPanel;
    //public GameObject buttonPrefab;
    //public Transform buttonContainer;
    //private List<GameObject> skillButtons = new List<GameObject>(); // 存放生成的按鈕
    //#endregion
    //
    //#region 事件註冊
    //private void OnEnable() {
    //    if (skillController != null)
    //    {
    //        skillController.OnSkillSlotUpdated += UpdateSkillPanel;
    //    }
    //}
    //
    //private void OnDisable() {
    //    if (skillController != null)
    //    {
    //        skillController.OnSkillSlotUpdated -= UpdateSkillPanel;
    //    }
    //}
    //#endregion
    //
    //#region Start
    //void Start() {
    //    StartCoroutine(DelayedSkillPanelUpdate());
    //}
    //
    //private IEnumerator DelayedSkillPanelUpdate() {
    //    yield return new WaitForSeconds(0.1f);
    //    UpdateSkillPanel(0, null);
    //}
    //#endregion
    //
    //#region 更新技能面板
    //public void UpdateSkillPanel(int slotIndex, PlayerSkillData.PlayerSkillStats skillStats) {
    //    foreach (Transform button in buttonContainer)
    //    {
    //        Destroy(button.gameObject);
    //    }
    //    skillButtons.Clear(); // 清除舊的按鈕記錄
    //
    //    for (int i = 0; i < 4; i++)
    //    {
    //        PlayerSkillData.PlayerSkillStats skill = skillController.GetSkillStats(i);
    //        GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
    //        TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
    //
    //        if (buttonText == null)
    //        {
    //            Debug.LogError("❌ [SkillPanel] buttonPrefab 內部缺少 TMP_Text 組件！");
    //            return;
    //        }
    //
    //        // **如果技能槽為空，則不顯示任何文字**
    //        buttonText.text = skill != null ? skill.skillName : "";
    //
    //        int index = i;
    //        newButton.GetComponent<Button>().onClick.AddListener(() => OnSkillButtonClick(index));
    //
    //        skillButtons.Add(newButton); // 記錄按鈕
    //    }
    //}
    //#endregion
    //
    //
    //#region 取得技能槽按鈕
    //public Transform GetSkillSlotButton(int slotIndex) {
    //    if (slotIndex < 0 || slotIndex >= skillButtons.Count)
    //    {
    //        Debug.LogError($"❌ [SkillPanel] GetSkillSlotButton() 查詢失敗，無效的技能槽索引 {slotIndex}！");
    //        return null;
    //    }
    //    return skillButtons[slotIndex].transform;
    //}
    //#endregion
    //
    //
    //#region 點擊技能按鈕
    //private void OnSkillButtonClick(int slotIndex) {
    //    if (skillSelectionPanel != null)
    //    {
    //        //skillSelectionPanel.ShowSelectionPanel(slotIndex, GetSkillSlotButton(slotIndex));
    //    }
    //    else
    //    {
    //        Debug.LogError("❌ [SkillPanel] skillSelectionPanel 未設定！");
    //    }
    //}
    //#endregion
}
