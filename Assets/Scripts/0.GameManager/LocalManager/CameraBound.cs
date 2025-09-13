using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraBound : MonoBehaviour
{
    public Collider2D collider2D;
    private void Start()
    {
        CameraManager.Instance.SetConfiner(collider2D);
    }

}
