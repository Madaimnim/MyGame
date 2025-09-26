using System.Collections;
using UnityEngine;
public sealed class CoroutineRunnerAdapter : ICoroutineRunner
{
    private readonly MonoBehaviour _host;
    public CoroutineRunnerAdapter(MonoBehaviour host) => _host = host;

    public Coroutine StartCoroutine(IEnumerator routine) => _host.StartCoroutine(routine);
}