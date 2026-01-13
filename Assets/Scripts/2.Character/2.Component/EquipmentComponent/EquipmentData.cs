public enum EquipmentSlot {
    Weapon,
    Armor,
    Accessory
}

[System.Serializable]
public class EquipmentData {
    public int Id;
    public string Name;
    public EquipmentSlot Slot;   // Weapon / Armor / Accessory
    public StatsData BonusStats;
}
