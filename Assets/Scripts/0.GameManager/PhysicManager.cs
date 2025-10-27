using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicManager : MonoBehaviour
{
    public static PhysicManager Instance { get; private set; }

    public PhysicConfig PhysicConfig;

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