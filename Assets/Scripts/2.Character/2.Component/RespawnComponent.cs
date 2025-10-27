using System;
using System.Collections;
using UnityEngine;

public class RespawnComponent 
{
    private MonoBehaviour _runner;
    public bool CanRespawn { get; private set; }
    public event Action OnRespawn;

    public RespawnComponent(MonoBehaviour runner,bool canRespawn) {
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        CanRespawn = canRespawn;
    }


    //延遲復活
    public void RespawnAfter(float delay) {
        if (!CanRespawn) return;
        _runner.StartCoroutine(DoRespawn(delay));
    }
    private IEnumerator DoRespawn(float delay) {
        yield return new WaitForSeconds(delay);
        Respawn();
    }

    public virtual void Respawn() {
        if (!CanRespawn) return;
        {
            //發事件
            OnRespawn.Invoke();
        }
    }

    //啟禁用AI、啟禁用復活
    public void EnableRespawn() => CanRespawn = true;
    public void DisableRespawn()=> CanRespawn = false;
}
