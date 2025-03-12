using System.Collections;
using UnityEngine;


public class Debugger : MonoBehaviour
{
    public BehaviorTree behaviorTree;

    void Start() {

    }
 
    #region OnGUI
    void OnGUI() {
        if (behaviorTree == null) {
            Debug.Log("OnGUI®Sß‰®ÏBehaviorTree");
            return;
        }

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        string debugInfo = $"BlackBoard Debug Info\n"
                         + $"----------------------\n"
                         + $"isDead: {behaviorTree.isDead}\n"                                 //isDead
                         + $"isHurt: {behaviorTree.isHurt}\n\n"                               //isHurt

                         + $"canMove: {behaviorTree.canMove}\n"                               //canMove
                         + $"isMoving: {behaviorTree.isMoving} \n"                            //isMoving
                         + $"canStartMove: {behaviorTree.canStartMove}\n\n"                   //canStartMove

                         + $"canAttack: {behaviorTree.isInAttack01Range}\n"                     //isInAttackRange
                         + $"canAttack: {behaviorTree.isInAttack02Range}\n"                     //isInAttackRange
                         + $"canAttack: {behaviorTree.isInAttack03Range}\n"                     //isInAttackRange
                         + $"canAttack: {behaviorTree.isInAttack04Range}\n"                     //isInAttackRange
                         + $"isCooldownComplete: {behaviorTree.isCooldownComplete}\n\n"         //isCooldownComplete

                         ;

        GUI.Label(new Rect(10, 10, 500, 250), debugInfo, style);
    }

    #endregion


}
