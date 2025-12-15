using System.Collections.Generic;
using UnityEngine;

public class TargetListManager:MonoBehaviour
{

    [SerializeField] private float updateInterval = 0.1f;
    public int GUIfontSize = 40;
    public Vector2 GUISize = new Vector2(500f, 200f);

    protected virtual Vector2 GUICornerOffset => new Vector2(10, 10); // 預設右上角
    protected virtual TextAnchor GUIAnchor => TextAnchor.UpperRight;  // 預設右上

    private float _timer;
    private readonly List<IInteractable> _targetList = new();
    public IReadOnlyList<IInteractable> TargetList => _targetList;

    private GUIStyle _cachedStyle;
    protected virtual void OnGUI() {
        if (_cachedStyle == null) {
            _cachedStyle = new GUIStyle(GUI.skin.label) {
                fontSize = GUIfontSize,
                normal = { textColor = Color.white },
                alignment = GUIAnchor
            };
        }

        float x = GUIAnchor == TextAnchor.UpperRight
            ? Screen.width - GUISize.x - GUICornerOffset.x
            : GUICornerOffset.x;

        float y = GUICornerOffset.y;

        GUI.Label(new Rect(x, y, GUISize.x, GUISize.y),
            $"{name}\nTarget Count: {_targetList.Count}",
            _cachedStyle);
    }


    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= updateInterval) {
            RemoveNullTargets();
            _timer = 0f;
        }
    }

    private void RemoveNullTargets()
    {
        _targetList.RemoveAll(i => i == null || i.Equals(null));
    }

    public void Register(IInteractable interactable){
        //Debug.Log($"註冊目標: {interactable}");
        if (!_targetList.Contains(interactable)) _targetList.Add(interactable);
    }
    public void Unregister(IInteractable interactable)=> _targetList.Remove(interactable); 
}
