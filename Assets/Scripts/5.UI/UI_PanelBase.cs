using UnityEngine;


//�~�Ӫ̡A���O���n�b��SetActive�A�I�s���}���Ҧb����}�_�B����
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
