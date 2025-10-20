using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicManager : MonoBehaviour
{
    public PhysicConfig PhysicConfig;
    public static PhysicManager Instance { get; private set; }

    private void Awake()
    {
        //³æ¨Ò
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
}