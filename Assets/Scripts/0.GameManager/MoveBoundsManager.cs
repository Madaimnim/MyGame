using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoundsManager : MonoBehaviour
{
    public static MoveBoundsManager Instance { get; private set; }
    private readonly List<BoxCollider2D> _boundsColliders = new();

    void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void ClearBounds() {
        _boundsColliders.Clear();
    }
    public void Register(BoxCollider2D col) {
        if (!_boundsColliders.Contains(col))
            _boundsColliders.Add(col);
    }


    public Vector2 Resolve(Vector2 desiredPos) {
        foreach (var col in _boundsColliders) {
            if (col.bounds.Contains(desiredPos)) {
                // 在禁止區域 → 推回合法位置
                desiredPos = PushOut(desiredPos, col.bounds);
            }
        }
        return desiredPos;
    }

    private Vector2 PushOut(Vector2 pos, Bounds b) {
        float left = Mathf.Abs(pos.x - b.min.x);
        float right = Mathf.Abs(b.max.x - pos.x);
        float bottom = Mathf.Abs(pos.y - b.min.y);
        float top = Mathf.Abs(b.max.y - pos.y);

        float min = Mathf.Min(left, right, bottom, top);

        if (min == left) pos.x = b.min.x;
        else if (min == right) pos.x = b.max.x;
        else if (min == bottom) pos.y = b.min.y;
        else pos.y = b.max.y;

        return pos;
    }
}
