public class EnemyOutlineManager {
    private static EnemyOutlineManager _instance;
    public static EnemyOutlineManager Instance => _instance ??= new EnemyOutlineManager();

    private Enemy _hoverEnemy;
    private Enemy _targetEnemy;

    private EnemyOutlineManager() { }

    //================ Hover =================
    public void SetHover(Enemy enemy) {
        if (_hoverEnemy == enemy) return;

        if (_hoverEnemy != null && _hoverEnemy != _targetEnemy)
            _hoverEnemy.EffectComponent.HideOutline();

        _hoverEnemy = enemy;

        if (_hoverEnemy != null && _hoverEnemy != _targetEnemy)
            _hoverEnemy.EffectComponent.ShowHoverOutline();
    }

    //================ Target =================
    public void SetTarget(Enemy enemy) {
        if (_targetEnemy == enemy) return;

        ClearTarget();

        _targetEnemy = enemy;
        if (_targetEnemy != null)
            _targetEnemy.EffectComponent.ShowTargetOutline();
    }

    public void ClearTarget() {
        if (_targetEnemy != null) {
            _targetEnemy.EffectComponent.HideOutline();
            _targetEnemy = null;
        }
    }

    public void ClearAll() {
        if (_hoverEnemy != null) {
            _hoverEnemy.EffectComponent.HideOutline();
            _hoverEnemy = null;
        }

        ClearTarget();
    }

    public Enemy CurrentTarget => _targetEnemy;
}
