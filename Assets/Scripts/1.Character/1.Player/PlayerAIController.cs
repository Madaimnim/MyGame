using UnityEngine;
using System.Collections.Generic;

public class PlayerAIController : MonoBehaviour,IMoveable
{
    [Header("AI¾ð§ó·sÀW²v")]
    public float updateInterval = 0.1f;
    private float updateTimer = 0f;
    private BehaviorTree behaviorTree;
    private Player player;



    private void Start() {
        player = GetComponent<Player>();
        behaviorTree = GetComponent<BehaviorTree>();
        SetBehaviorTree();
    }

    private void Update() {
        if (!player.canRunAI) return;
        RunBehaviorTree();
    }

    private void RunBehaviorTree() {
        if (updateTimer <= 0f)
        {
            behaviorTree.Tick();
            updateTimer = updateInterval;
        }
        updateTimer -= Time.deltaTime;
    }

    public void Move() { 
    
    }

    private void SetBehaviorTree() {
        behaviorTree.SetRoot(new Selector(new List<Node>
        {
            new Action_Attack(player, 4),
            new Action_Attack(player, 3),
            new Action_Attack(player, 2),
            new Action_Attack(player, 1),
            //new Action_Move(this),
            new Action_Idle()
        }));
    }
}
