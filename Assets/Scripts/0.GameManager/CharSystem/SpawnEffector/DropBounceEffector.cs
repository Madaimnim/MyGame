using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBounceSpawnEffector : IAppearEffector
{     
    public IEnumerator Play(GameObject ob, Vector3 finalPos) {
        Vector3 startPos = finalPos + new Vector3(0,5,0);
        ob.transform.position = startPos;

        //¸¨¦a
        float timer = 0f;
        const float dropDuration = 0.5f;
        while (timer<1f)
        {
            timer += Time.deltaTime / dropDuration;
            ob.transform.position = Vector3.Lerp(startPos, finalPos, timer * timer);
            yield return null;
        }

        //¼u¸õ
        Vector3 bouncePos = finalPos + new Vector3(0,0.5f,0);
        timer = 0;
        const float bounceDuration = 0.3f;
        while (timer < 1f)
        {
            timer += Time.deltaTime/bounceDuration;
            ob.transform.position = Vector3.Lerp(finalPos, bouncePos, Mathf.Sin(timer * Mathf.PI));
            yield return null;
        }
        ob.transform.position = finalPos;
    }
}
