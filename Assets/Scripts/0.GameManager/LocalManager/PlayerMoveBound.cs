using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveBound : MonoBehaviour
{
    public Collider2D meleeMovementBounds;
    public Collider2D rangedMovementBounds;

    private IEnumerator Start() {
        //Debug.Log("PlayerMoveBound.Start() 執行");
        yield return StartCoroutine(GameManager.Instance.WaitForDataReady());
        yield return new WaitForSeconds(1f);

        if (PlayerStateManager.Instance != null)
        {
            Debug.Log($" deployedPlayersDtny 數量：{PlayerStateManager.Instance.deployedPlayersDtny.Count}");
            foreach (var playerObj in PlayerStateManager.Instance.deployedPlayersDtny.Values)
            {
                if (playerObj == null)
                {
                    //Debug.LogWarning("deployedPlayersDtny沒有激活的角色");
                    continue;
                }

                PlayerMove playerMove = playerObj.GetComponent<PlayerMove>();
                if (playerMove == null)
                {
                    //Debug.LogWarning($"{playerObj.name} 上沒有 PlayerMove 腳本！");
                    continue;
                }

                playerMove.meleeMovementBounds = meleeMovementBounds;
                playerMove.rangedMovementBounds = rangedMovementBounds;
                //Debug.Log($"設定 {playerObj.name} 的移動邊界成功");
            }

        }
        else
        {
            Debug.LogWarning("PlayerStateManager不存在目前場景");
        }
            
    }
}
