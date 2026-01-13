using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {
    public int testPlayerId = 1001;
    public int skillSlot = 1;
    public float interval = 2f;
    [SerializeField]private MonoBehaviour _positionProvider;
    public IPositionProvider PositionProvider => _positionProvider as IPositionProvider;

    private IEnumerator Start() {
        // 等資料載完
        yield return new WaitUntil(() =>GameManager.Instance != null &&GameManager.Instance.IsAllDataLoaded);

        GameManager.Instance.PlayerStateSystem.PrepareInitialPlayers();
        var player = PlayerUtility.GetPlayer(testPlayerId);
        GameManager.Instance.PlayerStateSystem.PlayerAppear(player, PositionProvider.GetPosition(), AppearType.Instant);

        // 3 Presentation
        var host = player.GetComponent<PresentationHost>();
        host.SetController(player, new SkillPresentationController(skillSlot, interval));
    }
}
