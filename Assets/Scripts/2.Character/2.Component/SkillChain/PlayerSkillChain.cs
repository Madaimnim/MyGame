using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillChain 
{
    public int CurrentChain { get; private set; } = 0;

    private float _resetDelay = 1f;
    private Coroutine _resetRoutine;

    public void AddChain(MonoBehaviour runner) {
        CurrentChain++;
        // ­«±Ò­p®É
        if (_resetRoutine != null) runner.StopCoroutine(_resetRoutine);

        _resetRoutine = runner.StartCoroutine(ResetAfterDelay());
    }

    public void ResetChain() {
        CurrentChain = 0;
        _resetRoutine = null;
    }

    private IEnumerator ResetAfterDelay() {
        yield return new WaitForSeconds(_resetDelay);
        ResetChain();
    }
}
