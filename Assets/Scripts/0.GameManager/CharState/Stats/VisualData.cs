using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VisualData
{
    public GameObject CharPrefab;
    public Sprite SpriteIcon;
    public AudioClip DeathSFX;
    public Material NormalMaterial;
    public Material FlashMaterial;

    public VisualData(VisualData other) {
        CharPrefab = other.CharPrefab;
        SpriteIcon = other.SpriteIcon;
        DeathSFX = other.DeathSFX;
        NormalMaterial=other.NormalMaterial;
        FlashMaterial = other.FlashMaterial;
    }
}
