using System;
using System.Collections;
using UnityEngine;

public class CharRespawnComponent 
{
    protected MonoBehaviour _owner;
    public bool CanRespawn { get; private set; }
    public event Action OnRespawn;

    public CharRespawnComponent(MonoBehaviour owner) {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    public virtual void Respawn() {
        if (!CanRespawn) return;
        {
            //發事件
            OnRespawn.Invoke();
        }
    }
    //延遲復活
    public void RespawnAfter(float delay) {
        if (!CanRespawn) return;
        _owner.StartCoroutine(DoRespawn(delay));
    }
    private IEnumerator DoRespawn(float delay) {
        yield return new WaitForSeconds(delay);
        Respawn();
    }


    //啟禁用AI、啟禁用復活
    public void EnableRespawn() => CanRespawn = true;
    public void DisableRespawn()=> CanRespawn = false;
}
