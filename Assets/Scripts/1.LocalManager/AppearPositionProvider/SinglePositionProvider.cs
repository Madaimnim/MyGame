using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePositionProvider : MonoBehaviour, IPositionProvider {
    public Vector3 GetPosition() => transform.position;
}