using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRandomPositionProvider : MonoBehaviour, IPositionProvider {
    private BoxCollider2D _area;

    private void Awake() {
        _area = GetComponent<BoxCollider2D>();
        if (_area == null) Debug.LogError($"{name} ¯Ê¤Ö BoxCollider2D");
    }
    public Vector3 GetPosition() {
        var bounds = _area.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, 0f);
    }
}
