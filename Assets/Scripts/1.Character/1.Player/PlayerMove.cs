using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    private Player player;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
    }

    public void Move(Vector2 direction) {
        if (player == null || player.playerStats == null) return;
        if (player.isKnockback) return;

        //Debug.Log($"腳色移動方向為{direction}, 速度={player.playerStats?.moveSpeed}");
        // 直接計算移動位置，不再使用 Bounds 限制
        Vector2 newPosition = rb.position + direction * player.playerStats.moveSpeed * Time.fixedDeltaTime;

        // 用 Rigidbody2D 物理方式移動，會自動和其他 Collider2D 發生碰撞
        rb.MovePosition(newPosition);
    }

}
