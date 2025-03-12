using UnityEngine;
using UnityEngine.UI;

public class UIMenuController : MonoBehaviour
{
    public Button statusButton;
    public Button jobButton;
    public Button equipmentButton;
    public Button skillsButton;
    public Button formationButton;
    public Button battleButton;

    private void OnEnable() {
        statusButton.onClick.AddListener(OpenStatusPanel);
        jobButton.onClick.AddListener(OpenJobPanel);
        equipmentButton.onClick.AddListener(OpenEquipmentPanel);
        skillsButton.onClick.AddListener(OpenSkillsPanel);
        formationButton.onClick.AddListener(OpenFormationPanel);
        battleButton.onClick.AddListener(StartBattle);
    }

    private void OnDisable() {
        statusButton.onClick.RemoveListener(OpenStatusPanel);
        jobButton.onClick.RemoveListener(OpenJobPanel);
        equipmentButton.onClick.RemoveListener(OpenEquipmentPanel);
        skillsButton.onClick.RemoveListener(OpenSkillsPanel);
        formationButton.onClick.RemoveListener(OpenFormationPanel);
        battleButton.onClick.RemoveListener(StartBattle);
    }

    // 這些方法與按鈕綁定
    private void OpenStatusPanel() => UIManager.Instance.OpenUIPanel(UIManager.Instance.statusUIPanel);
    private void OpenJobPanel() => UIManager.Instance.OpenUIPanel(UIManager.Instance.jobUIPanel);
    private void OpenEquipmentPanel() => UIManager.Instance.OpenUIPanel(UIManager.Instance.equipmentUIPanel);
    private void OpenSkillsPanel() => UIManager.Instance.OpenUIPanel(UIManager.Instance.skillsUIPanel);
    private void OpenFormationPanel() => UIManager.Instance.OpenUIPanel(UIManager.Instance.formationUIPanel);
    private void StartBattle() => GameStateManager.Instance.SetState(GameStateManager.GameState.Battle);

}
