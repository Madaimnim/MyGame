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

                         ;

        GUI.Label(new Rect(10, 10, 500, 250), debugInfo, style);
    }

    #endregion


}
