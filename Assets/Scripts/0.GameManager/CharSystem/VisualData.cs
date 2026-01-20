using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VisualData
{
    public GameObject Prefab;
    public Sprite SpriteIcon;




    public VisualData(VisualData other) {
        Prefab = other.Prefab;
        SpriteIcon = other.SpriteIcon;



    }
}
