using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Player player;
    [Header("移動限制範圍（依角色類型）")]
    public Collider2D meleeMovementBounds;
    public Collider2D rangedMovementBounds;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    public void Move(Vector2 direction) {
        if (player == null || player.playerStats == null) return;
        if (player.isKnockback) return;

        Collider2D currentBounds =
            player.playerStats.playerType == PlayerStatData.PlayerStatsTemplate.PlayerType.Melee
            ? meleeMovementBounds
            : rangedMovementBounds;

        if (currentBounds == null)
            return;

        Vector2 newPosition = rb.position + direction * player.playerStats.moveSpeed * Time.fixedDeltaTime;
        Bounds bounds = currentBounds.bounds;

        newPosition.x = Mathf.Clamp(newPosition.x, bounds.min.x, bounds.max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, bounds.min.y, bounds.max.y);

        rb.MovePosition(newPosition);
    }

}
