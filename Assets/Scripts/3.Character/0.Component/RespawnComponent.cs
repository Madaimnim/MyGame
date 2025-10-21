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


    //����_��
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
            //�o�ƥ�
            OnRespawn.Invoke();
        }
    }

    //�ҸT��AI�B�ҸT�δ_��
    public void EnableRespawn() => CanRespawn = true;
    public void DisableRespawn()=> CanRespawn = false;
}
