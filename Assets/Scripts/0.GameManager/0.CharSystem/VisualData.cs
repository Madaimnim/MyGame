using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VisualData
{
    public GameObject Prefab;
    public GameObject DetectPrefab;

    public Sprite SpriteIcon;

    public AudioClip DeathSFX;

    public Material NormalMaterial;
    public Material FlashMaterial;

    public VisualData(VisualData other) {
        Prefab = other.Prefab;
        DetectPrefab = other.DetectPrefab;

        SpriteIcon = other.SpriteIcon;

        DeathSFX = other.DeathSFX;

        NormalMaterial=other.NormalMaterial;
        FlashMaterial = other.FlashMaterial;
    }
}
