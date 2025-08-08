using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// ステージ管理クラス
/// </summary>
public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField]
    private Rect _area;
    public Rect PlayArea
        => _area;

    [SerializeField]
    private float _spawnInterval;

    [SerializeField]
    private EnemySpawner _spawner;

    private List<EnemySpawner> _createdSpawner;

    [SerializeField]
    private SpawnEnemyTable _spawnTable;

    [SerializeField]
    private EnemyEnums.EnemyID[] _middleBosses;

    private float _remainInterval = 0;

    private float _bossInterval = 60.0f;

    private int _bigBossCount = 2;

    public void Initialize()
    {
        _createdSpawner = new();

        _bigBossCount = 2;

        CreateSpawner(1.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (EnemyManager.Instance.IsBossMode)
            return;

        if (_bossInterval < 0)
        {
            if (_bigBossCount > 0)
            {
                EnemyManager.Instance.CreateEnemy((int)_middleBosses[0], GetRandomPositionInArea());
                --_bigBossCount;
                _bossInterval = 60.0f;
                CreateSpawner(8.0f);
            }
            else
            {
                EventDispatcher.Instance.Dispatch("BossEvent");
                _bossInterval = 60.0f;
            }
        }
        else
            _bossInterval -= Time.fixedDeltaTime;
    }

    public void CreateSpawner(float interval)
    {
        var spawner = Instantiate(_spawner, this.transform.position, Quaternion.identity);
        _createdSpawner.Add(spawner);

        var idx = Mathf.Min(_createdSpawner.Count - 1, _createdSpawner.Count);

        spawner.Initialize(_spawnTable.Table[idx], UsableMethods.GetRandomDirection2D(), PlayArea, interval);
    }

    public Vector3 GetRandomPositionInArea(float minDistanceFromPlayer = 0, float maxDistanceFromPlayer = 0)
    {
        ITargetProvider player = PlayerManager.Instance.Player;
        Vector3 spawnPos;
        int safetyLoop = 0;

        do
        {
            // Rect内のランダム座標を取得
            float x = Random.Range(PlayArea.xMin, PlayArea.xMax);
            float y = Random.Range(PlayArea.yMin, PlayArea.yMax);
            spawnPos = new Vector3(x, y, 0);

            safetyLoop++;
            if (safetyLoop > 999)
            {
                spawnPos = Vector3.zero;
                break; // 無限ループ対策
            }
        } while (Vector3.Distance(spawnPos, player.GetPostion()) < minDistanceFromPlayer || Vector3.Distance(spawnPos, player.GetPostion()) > maxDistanceFromPlayer);

        return spawnPos;
    }
}
