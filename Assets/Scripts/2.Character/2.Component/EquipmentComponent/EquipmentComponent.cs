using System.Collections.Generic;

public class EquipmentComponent {
    private Dictionary<EquipmentSlot, EquipmentData> _equipSlotDtny = new Dictionary<EquipmentSlot, EquipmentData>();

    private StatsComponent _statsComponent;

    public EquipmentComponent(StatsComponent statsComponent) {
        _statsComponent = statsComponent;
    }

    public void Equip(EquipmentData data) {
        _equipSlotDtny[data.Slot] = data;
        Recalculate();
    }

    public void Unequip(EquipmentSlot slot) {
        if (_equipSlotDtny.Remove(slot))
            Recalculate();
    }

    private void Recalculate() {
        // 先清空裝備加成
        _statsComponent.BonusStats.Power = 0;
        _statsComponent.BonusStats.MoveSpeed = 0;
        _statsComponent.BonusStats.KnockbackPower = 0;
        _statsComponent.BonusStats.FloatPower = 0;
        _statsComponent.BonusStats.Weight = 0;

        // 疊加所有裝備
        foreach (var equipmentData in _equipSlotDtny.Values) {
            var bonusStatsData = equipmentData.BonusStats;
            _statsComponent.BonusStats.Power += bonusStatsData.Power;
            _statsComponent.BonusStats.MoveSpeed += bonusStatsData.MoveSpeed;
            _statsComponent.BonusStats.KnockbackPower += bonusStatsData.KnockbackPower;
            _statsComponent.BonusStats.FloatPower += bonusStatsData.FloatPower;
            _statsComponent.BonusStats.Weight += bonusStatsData.Weight;
        }

        _statsComponent.RecalculateFinalStats();
    }
}
