using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test02 : MonoBehaviour
{
    public int a=0;
    public bool a_isAlready=false;

    void Awake() {
        a = 10;
        a_isAlready = true;

        Debug.Log("Test02 Awake");

    }
    void OnEnable() {
        Debug.Log("Test02 OnEnable");

    }


    void Start() {
        Debug.Log("Test02  Start");
    }


    void Update() {
        Debug.Log("Test02  Update");
    }
}
