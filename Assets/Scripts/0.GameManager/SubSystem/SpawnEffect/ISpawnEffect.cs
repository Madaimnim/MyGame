using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawnEffect 
{
    IEnumerator Play(GameObject ob,Vector3 finalPos);
}
