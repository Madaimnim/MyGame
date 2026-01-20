using UnityEngine;
using System.Collections.Generic;

//優先執行
[DefaultExecutionOrder(-50)]
public class MultiPositionProvider : MonoBehaviour, IPositionProvider {
    [Header("生成點（場景中放空物件）")]
    [SerializeField] private List<Transform> _points = new List<Transform>();

    [Header("是否隨機")]
    [SerializeField] private bool _isRandom = false;

    private int _index = 0;
    private void Awake() {

    }

    private void OnEnable() {
        _index = 0;
        _points.Clear();
        foreach (Transform child in transform) _points.Add(child);
    }

    public Vector3 GetPosition() {
        if (_points == null || _points.Count == 0) {
            Debug.LogError($"{name} 沒有設定任何生成點");
            return transform.position;
        }

        if (_isRandom) {
            int i = Random.Range(0, _points.Count);
            return _points[i].position;
        }
        else {
            var pos = _points[_index].position;
            _index = (_index + 1) % _points.Count;
            return pos;
        }
    }
}
