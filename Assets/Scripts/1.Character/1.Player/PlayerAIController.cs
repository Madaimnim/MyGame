using UnityEngine;
using System.Collections.Generic;

public class PlayerAIController : MonoBehaviour
{
    private BehaviorTree behaviorTree;
    private Player player;

    private float behaviorTreeUpdateCooldown = 0f;
    private float behaviorTreeUpdateInterval = 0.1f;        //每1秒更新一次AI樹

    private void Start() {
        player = GetComponent<Player>();
        behaviorTree = GetComponent<BehaviorTree>();
        SetBehaviorTree();
    }

    private void Update() {
        RunBehaviorTree();
    }

    private void RunBehaviorTree() {
        if (behaviorTreeUpdateCooldown <= 0f)
        {
            behaviorTree.Tick();
            behaviorTreeUpdateCooldown = behaviorTreeUpdateInterval;
        }
        behaviorTreeUpdateCooldown -= Time.deltaTime;
    }

    private void SetBehaviorTree() {
        behaviorTree.SetRoot(new Selector(new List<Node>
        {
            new Action_Attack(player, 4),
            new Action_Attack(player, 3),
            new Action_Attack(player, 2),
            new Action_Attack(player, 1),
            new Action_Move(),
            new Action_Idle()
        }));
    }
}
