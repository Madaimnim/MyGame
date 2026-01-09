using UnityEngine;

public class SkillPresentationController : IPresentationController {
    private Player _player;
    private int _skillSlot;
    private float _interval;
    private float _timer;

    public SkillPresentationController(int skillSlot, float interval) {
        _skillSlot = skillSlot;
        _interval = interval;
    }

    public void Enter(Player player) {
        _player = player;
        // 顯示技能範圍
        _player.SkillComponent.SetDetectRangeVisible(_skillSlot, true);

        _timer = 0f;
    }

    public void Tick() {
        _timer += Time.deltaTime;
        if (_timer >= _interval) {
            _timer = 0f;

            // Todo只播放技能動畫，不走戰鬥邏輯
            //_player.SkillComponent.PlaySkillPreview(_skillSlot);
        }
    }

    public void Exit() {
        _player.SkillComponent.SetDetectRangeVisible(_skillSlot, false);
    }
}
