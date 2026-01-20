using UnityEngine;

public class EnergySlotView : MonoBehaviour {
    [Header("Image")]
    public Transform partial;   // ¶ê²y
    public GameObject full;     // ¤õµK


    //value: 0 ~ 1
    public void SetPartial(float value) {
        value = Mathf.Clamp01(value);

        full.SetActive(false);
        partial.gameObject.SetActive(true);
        partial.localScale = Vector3.one * value;
    }

    public void SetFull() {
        partial.gameObject.SetActive(false);
        full.SetActive(true);
    }

    public void SetEmpty() {
        full.SetActive(false);
        partial.gameObject.SetActive(false);
    }
}
