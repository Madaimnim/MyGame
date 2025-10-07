using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;


public class EnemySpawnSystem
{
    private DefaultEnemyFactory _factory;
    private ISpawnEffect _spawnEffect;
    private ICoroutineRunner _runner;

    public event Action<Enemy> OnEnemySpawned;

    public EnemySpawnSystem(DefaultEnemyFactory factory, ISpawnEffect spawnEffect, ICoroutineRunner runner) {
        _factory = factory;
        _spawnEffect = spawnEffect;
        _runner = runner;
    }

    public Enemy SpawnEnemy(int id,Vector3 spawnPos) {
        var enemy = _factory.CreateEnemy(id);
        _runner.StartCoroutine(_spawnEffect.Play(enemy.gameObject, spawnPos));

            //µo¨Æ¥ó
         OnEnemySpawned?.Invoke(enemy);
        return enemy;
    }

}
