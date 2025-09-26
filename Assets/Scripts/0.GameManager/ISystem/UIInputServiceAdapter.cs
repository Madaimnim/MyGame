public class UIInputServiceAdapter : IUIInputService
{
    public void Enable() => UIController_Input.Instance.isUIInputEnabled = true;
    public void Disable() => UIController_Input.Instance.isUIInputEnabled = false;
}