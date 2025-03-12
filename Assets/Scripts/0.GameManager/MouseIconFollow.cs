using UnityEngine;
using UnityEngine.UI;

public class MouseIconFollow : MonoBehaviour
{
    public RectTransform iconRectTransform; // 拖入 UI 圖像
    public Canvas canvas; // 拖入 Canvas
    public Vector2 offset = new Vector2(10f, -10f); // 微調偏移量

    void Update() {
        if (iconRectTransform != null)
        {
            Vector2 mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out mousePosition
            );

            iconRectTransform.anchoredPosition = mousePosition + offset;
        }
    }
}
