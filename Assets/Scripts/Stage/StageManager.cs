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

    [SerializeField]
    private GameObject _bossSummonParticle;

    [SerializeField]
    private ParticleSystem _bossSummonEffect;

    private WaveController _waveController;

    [SerializeField]
    private TextWrapper _timerText;
    private float _timerCounter;

    private float _remainInterval = 0;

    private float _bossInterval = 60.0f;

    private int _bigBossCount = 2;

    public void Initialize()
    {
        _createdSpawner = new();

        _waveController = new();

        _bigBossCount = 2;

        _timerText.Initialize();
        _timerCounter = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _waveController.Update();

        if (EnemyManager.Instance.IsBossMode)
        {
            _timerCounter = 300;
            int minutes = (int)(_timerCounter / 60);
            int seconds = (int)(_timerCounter % 60);

            _timerText.SetText($"{minutes:00}:{seconds:00}");
            return;
        }
        UpdateTimerUI();   
    }

    private void UpdateTimerUI()
    {
        _timerCounter += Time.fixedDeltaTime;
        int minutes = (int)(_timerCounter / 60);
        int seconds = (int)(_timerCounter % 60);

        _timerText.SetText($"{minutes:00}:{seconds:00}");
    }

    public void CreateSpawner(float interval)
    {
        var spawner = Instantiate(_spawner, this.transform.position, Quaternion.identity);
        _createdSpawner.Add(spawner);

        var idx = Mathf.Min(_createdSpawner.Count - 1, _createdSpawner.Count);

        spawner.Initialize(_spawnTable.Table[idx], UsableMethods.GetRandomDirection2D(), PlayArea, interval);
    }

    public Vector3 GetRandomPositionInArea()
    {
        ITargetProvider player = PlayerManager.Instance.Player;
        Vector3 spawnPos;
        int safetyLoop = 0;

        // Rect内のランダム座標を取得
        float x = Random.Range(PlayArea.xMin, PlayArea.xMax);
        float y = Random.Range(PlayArea.yMin, PlayArea.yMax);
        spawnPos = new Vector3(x, y, 0);

        return spawnPos;
    }
}
