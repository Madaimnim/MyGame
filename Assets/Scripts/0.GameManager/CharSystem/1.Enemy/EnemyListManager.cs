using System.Collections.Generic;
using UnityEngine;

public struct EnemyInfo
{
    public Enemy Enemy;
    public Vector2 BottomPosition;
    public Vector2 SprPosition;
}

public class EnemyListManager:MonoBehaviour
{
    public static EnemyListManager Instance { get; private set; }

    [SerializeField] private float updateInterval = 0.1f;
    public int GUIfontSize = 20;
    public Vector2 GUISize = new Vector2(250f, 30f);
    private void OnGUI()
    {

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = GUIfontSize;
        style.normal.textColor = Color.white; // 字體顏色

        // 計算右上角位置
        float x = Screen.width - GUISize.x - 10; // 離右邊 10px
        float y = 10f; // 離上方 10px

        GUI.Label(new Rect(x, y, GUISize.x, GUISize.y), $"Enemy Count: {_enemyInfoList.Count}", style);
    }

    private float _timer;
    private readonly List<EnemyInfo> _enemyInfoList = new();
    public IReadOnlyList<EnemyInfo> EnemyInfoList => _enemyInfoList;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        UpdateInfoListCache();
    }

    public void Register(Enemy e)
    {
        if (_enemyInfoList.Exists(i => i.Enemy == e)) return;

        _enemyInfoList.Add(new EnemyInfo
        {
            Enemy = e,
            BottomPosition = e.transform.position,
            SprPosition = e.Spr.transform.position
        });
    }
    public void Unregister(Enemy e)
    {
        _enemyInfoList.RemoveAll(i => i.Enemy == e);
    }

    public void UpdateInfoListCache()
    {
        if (_enemyInfoList.Count == 0) return;

        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;
        _timer = 0f;

        for (int i = 0; i < _enemyInfoList.Count; i++)
        {
            if (_enemyInfoList[i].Enemy == null)
            {
                _enemyInfoList.RemoveAt(i);
                i--;
                continue;
            }

            var info = _enemyInfoList[i];
            info.BottomPosition = info.Enemy.transform.position;
            info.SprPosition = info.Enemy.Spr.transform.position;
            _enemyInfoList[i] = info;
        }
    }
}
