using System.Collections;
using UnityEngine;
public sealed class CoroutineRunnerAdapter : ICoroutineRunner
{
    private readonly MonoBehaviour _host;

    public CoroutineRunnerAdapter(MonoBehaviour host)
    {
        _host = host;
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return _host.StartCoroutine(routine);
    }

    public void StopCoroutine(Coroutine routine)
    {
        _host.StopCoroutine(routine);
    }

}