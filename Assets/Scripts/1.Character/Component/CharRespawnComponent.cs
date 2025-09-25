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
            OnRespawn.Invoke();
    }

    //©µ¿ð´_¬¡
    public void RespawnAfter(float delay) {
        if (!CanRespawn) return;
        _owner.StartCoroutine(DoRespawn(delay));
    }
    private IEnumerator DoRespawn(float delay) {
        yield return new WaitForSeconds(delay);
        Respawn();
    }


    //±Ò¸T¥ÎAI¡B±Ò¸T¥Î´_¬¡
    public void EnableRespawn() => CanRespawn = true;
    public void DisableRespawn()=> CanRespawn = false;
}
