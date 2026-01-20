using UnityEngine;

public class EnergyUIController : MonoBehaviour {
    [Header("Slots")]
    public EnergySlotView[] slots;

    private EnergyComponent _energyComponent;

    //給 Player 初始化時呼叫

    public void Bind(EnergyComponent energy) {
        _energyComponent = energy;
        _energyComponent.OnEnergyChanged += Refresh;

        Refresh(_energyComponent.CurrentEnergy, _energyComponent.MaxEnergy);
    }

    private void OnDestroy() {
        if (_energyComponent != null)
            _energyComponent.OnEnergyChanged -= Refresh;
    }

    private void Refresh(float current, float max) {
        int fullCount = Mathf.FloorToInt(current);
        float partial = current - fullCount;
        //Debug.Log("刷新EnergyUI");

        for (int i = 0; i < slots.Length; i++) {
            if (i < fullCount) slots[i].SetFull();          
            else if (i == fullCount && partial > 0f) 
                slots[i].SetPartial(partial);
            else 
                slots[i].SetEmpty();
        }
    }
}
