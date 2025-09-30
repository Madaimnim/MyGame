using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;

public class PlayerSpawnSystem
{
    public IReadOnlyDictionary<int, GameObject> BattleObjects => _battleObjects;

    private readonly Dictionary<int, GameObject> _battleObjects = new();
    private readonly IPlayerFactory _factory;
    private readonly ISpawnEffect _spawnEffect;
    private readonly ICoroutineRunner _runner;
    private readonly Transform _parent;

    public PlayerSpawnSystem(IPlayerFactory factory, ISpawnEffect spawnEffect, Transform parent, ICoroutineRunner runner) {
        _factory = factory;
        _spawnEffect = spawnEffect;
        _parent = parent;
        _runner = runner;
    }

    public GameObject SpawnBattlePlayer(PlayerStatsRuntime rt) {
        var go = _factory.CreatPlayer(rt, _parent);
        _battleObjects[rt.StatsData.Id] = go;
        return go;
    }

    public void DeactivateAll() {
        foreach (var go in _battleObjects.Values)
        {
            if (!go) continue;
            go.SetActive(false);
            go.transform.position = Vector2.zero;
        }
    }

    //事件方法
    public void ActivateAll(Vector3 spawnPos) {
        float offsetY = -1f;
        foreach (var go in _battleObjects.Values)
        {
            if (!go) continue;
            go.SetActive(true);
            _runner.StartCoroutine(_spawnEffect.Play(go, spawnPos));
            spawnPos.y += offsetY;
        }
    }
}
