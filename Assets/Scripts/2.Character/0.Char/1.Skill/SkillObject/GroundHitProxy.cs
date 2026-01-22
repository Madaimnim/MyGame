using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundHitProxy : MonoBehaviour {
    public event Action<Collider2D> OnGroundHit;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareLayer("Enemy")) return;
        OnGroundHit?.Invoke(other);
    }
}
