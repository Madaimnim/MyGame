using UnityEngine;


//繼承者，切記不要在用SetActive，呼叫此腳本所在物件開起、關閉
public abstract class UI_PanelBase : MonoBehaviour
{

    protected virtual void OnEnable() {
        UIManager.Instance.RegisterPanel(this);
        Refresh();
    }


    public virtual void Show() {
        gameObject.SetActive(true);
       
    }
    public virtual void Hide() {
        gameObject.SetActive(false);
    }
    public abstract void Refresh();
}
