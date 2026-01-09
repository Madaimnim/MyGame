using UnityEngine;
using System.Collections.Generic;

public enum Rarity {
    Normal,     // ¥Õ
    Uncommon,   // ºñ
    Rare,       // ÂÅ
    Epic,       // µµ
    Legendary,  // ª÷
    Mythic      // ¬õ
}
public static class RarityColor {
    private static readonly Dictionary<Rarity, Color> _map =
        new Dictionary<Rarity, Color> {
            { Rarity.Normal,     Color.white },
            { Rarity.Uncommon,   new Color(0.3f, 1f, 0.3f) }, // ºñ
            { Rarity.Rare,       new Color(0.3f, 0.6f, 1f) }, // ÂÅ
            { Rarity.Epic,       new Color(0.7f, 0.4f, 1f) }, // µµ
            { Rarity.Legendary,  new Color(1f, 0.8f, 0.2f) }, // ª÷
            { Rarity.Mythic,     new Color(1f, 0.2f, 0.2f) }, // ¬õ
        };

    public static Color Get(Rarity rarity)
        => _map[rarity];
}
