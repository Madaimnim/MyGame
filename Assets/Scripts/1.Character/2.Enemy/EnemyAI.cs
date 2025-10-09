using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    private Enemy enemy;
    private EnemyStatsRuntime _rt;
    public float stopMoveDragPower;
    public Transform currentMoveTarget { get; private set; }

    public MoveStrategyBase moveStrategy;


    private void Awake() {
        enemy = GetComponent<Enemy>();
        _rt = enemy.Rt;
    }

    //public void StartMoving() {
    //    enemy.Rb.drag = 0;
    //    enemy.Rb.velocity = new Vector2(0, 0);
    //    Vector2 direction = moveStrategy.MoveDirection();
    //    float speed = enemy.Rt.StatsData.MoveSpeed;
    //    enemy.Rb.AddForce(new Vector2(direction.x * speed, direction.y * speed), ForceMode2D.Impulse);
    //}
    //public void StopMoving() {
    //    enemy.Rb.drag = stopMoveDragPower; // 設定較大的拖曳力，使角色自然減速
    //}


}