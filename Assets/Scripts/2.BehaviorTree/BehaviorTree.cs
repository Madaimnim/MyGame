using System;
using UnityEngine;

public class BehaviorTree
{
    private Node rootNode;
    public void SetRoot(Node root) {
        rootNode = root;
    }

    public void Tick() {
        rootNode?.Evaluate();
    }
}