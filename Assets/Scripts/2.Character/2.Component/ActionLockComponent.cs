using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;
public class ActionLockComponent {
    
    public int _lockCount = 0;
    private MonoBehaviour _runner;
    private StateComponent _stateComponent;


    public ActionLockComponent(MonoBehaviour runner,StateComponent stateComponent) {
        _runner = runner;
        _stateComponent= stateComponent;
    }
    public void Tick() {
        if (_lockCount > 0) _stateComponent.SetIsControlLocked(true);
        else _stateComponent.SetIsControlLocked(false);
    }

    public void HurtLock() {
        LockAction(0.5f);
    }

    //上鎖動作
    private void LockAction(float duration) {
        _lockCount++;
        _runner.StartCoroutine(UnlockAfter(duration));
    }
    IEnumerator UnlockAfter(float duration) {
        yield return new WaitForSeconds(duration);
        _lockCount = Mathf.Max(0, _lockCount - 1);
    }
    //強制解鎖
    public void ForceUnlock() {
        bool wasLocked = _lockCount > 0;
        _lockCount = 0;
    }
}