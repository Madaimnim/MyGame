using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    private Collider2D col;
    private void Awake() {
        col = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        CameraManager.Instance.SetConfiner(col);

    }

}
