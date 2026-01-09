using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AppearType {
    Instant,        // 直接出現（預設）
    DropBounce,     // 彈跳

}
public interface IAppearEffector
{
    IEnumerator Play(GameObject ob,Vector3 finalPos);
}
