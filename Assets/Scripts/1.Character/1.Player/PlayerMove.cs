using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public void Move(Vector2 direction,Player player,Rigidbody2D rb) {
        if (player.StatsRuntime == null) return;

        // 直接計算移動位置，不再使用 Bounds 限制
        Vector2 newPosition = rb.position + direction * player.StatsRuntime.MoveSpeed * Time.fixedDeltaTime;

        // 用 Rigidbody2D 物理方式移動，會自動和其他 Collider2D 發生碰撞
        rb.MovePosition(newPosition);
    }

}
