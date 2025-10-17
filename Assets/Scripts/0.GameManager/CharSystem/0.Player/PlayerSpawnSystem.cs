using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;

public class PlayerSpawnSystem
{
    private DefaultPlayerFactory _factory;
    private ISpawnEffect _spawnEffect;
    private ICoroutineRunner _runner;
    //事件
    public event Action<int> OnPlayerSpawned;

    public PlayerSpawnSystem(DefaultPlayerFactory factory, ISpawnEffect spawnEffect, ICoroutineRunner runner) {
        _factory = factory;
        _spawnEffect = spawnEffect;
        _runner = runner;
    }

    public Player CreatPlayer(int id) {
        var player = _factory.CreatPlayer(id);
        player.Rt.BattleObject = player.gameObject;

        return player;
    }

    public void SpawnAllPlayer(Vector3 spawnPos) {
        float offsetY = -1f;
        foreach (var player in PlayerUtility.AllPlayers.Values)
        {
            if (!player) continue;
            player.gameObject.SetActive(true);
            _runner.StartCoroutine(_spawnEffect.Play(player.gameObject, spawnPos));
            spawnPos.y += offsetY;

            //發事件
            OnPlayerSpawned?.Invoke(player.Rt.StatsData.Id);
        }
    }
    public void CloseAllPlayer() {
        foreach (var player in PlayerUtility.AllPlayers.Values)
        {
            if (!player) continue;
            player.gameObject.SetActive(false);
            player.transform.position = Vector2.zero;
        }
    }
}
