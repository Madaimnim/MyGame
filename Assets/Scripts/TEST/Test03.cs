using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class Test03 : MonoBehaviour
{
    void Awake() {
        Debug.Log("Test03 Awake");
    }
    void Start() {
        Debug.Log("Test03  Start");
    }
    private void OnEnable() {
        Debug.Log("Test03  OnEnable");
    }
}


