using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using System.Collections;
public class ActionLockComponent {
    public bool IsLocked => _lockCount > 0;

    private int _lockCount = 0;
    MonoBehaviour _runner;

    public event Action OnUnlocked;

    public ActionLockComponent(MonoBehaviour runner) {
        _runner = runner;
    }
    public void LockAction(float duration) {
        _lockCount++;
        _runner.StartCoroutine(UnlockAfter(duration));
    }

    IEnumerator UnlockAfter(float duration) {
        yield return new WaitForSeconds(duration);
        int prev = _lockCount;
        _lockCount = Mathf.Max(0, _lockCount - 1);

        if (prev > 0 && _lockCount == 0)  
            OnUnlocked?.Invoke();  
    }

    public void ForceUnlock() {
        bool wasLocked = _lockCount > 0;
        _lockCount = 0;

        if (wasLocked) 
            OnUnlocked?.Invoke();     
    }
}