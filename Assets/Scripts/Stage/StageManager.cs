using UnityEngine;
using System.Linq;

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
    private EnemyEnums.EnemyID[] _spawnableEnemys;

    private float _remainInterval = 0;

    public void Initialize()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_remainInterval < 0)
        {
            var id = _spawnableEnemys[Random.Range(0, _spawnableEnemys.Length)];

            EnemyManager.Instance.CreateEnemy((int)id, GetRandomPositionInArea(8));
            _remainInterval = _spawnInterval;
        }
        else
            _remainInterval -= Time.fixedDeltaTime;
    }

    public Vector3 GetRandomPositionInArea(float minDistanceFromPlayer = 0)
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
            if (safetyLoop > 100) break; // 無限ループ対策

        } while (Vector3.Distance(spawnPos, player.GetPostion()) < minDistanceFromPlayer);

        return spawnPos;
    }

}
