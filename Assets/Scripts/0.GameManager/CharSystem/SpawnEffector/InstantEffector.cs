using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InstantSpawnEffector : IAppearEffector {
    public IEnumerator Play(GameObject ob, Vector3 finalPos) {
        ob.transform.position = finalPos;
        yield break;
    }
}
