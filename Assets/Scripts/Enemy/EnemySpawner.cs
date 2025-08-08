using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    private SpawnTable _spawnableEnemys;

    private Vector3 _moveDir;

    private Rect _playArea;

    private float _spawnInterval;

    private float _remainInterval;

    public void Initialize(SpawnTable spawnableEnemys, Vector3 firstDir, Rect playArea, float spawnInterval)
    {
        _spawnableEnemys = spawnableEnemys;
        _moveDir = firstDir;
        _spawnInterval = spawnInterval;
        _remainInterval = _spawnInterval;
        _playArea = playArea;
    }

    private void FixedUpdate()
    {
        Move();

        _remainInterval -= Time.fixedDeltaTime;

        if (_remainInterval > 0 || EnemyManager.Instance.IsBossMode)
            return;

        var id = _spawnableEnemys.Table[Random.Range(0, _spawnableEnemys.Table.Length)];

        EnemyManager.Instance.CreateEnemy((int)id, this.transform.position, UsableMethods.GetRandomDirection2D());
        _remainInterval = _spawnInterval;
    }

    private void Move()
    {
        var pos = this.transform.position;
        var scale = this.transform.localScale;
        var halfScale = scale * 0.5f;

        pos += _moveDir * _moveSpeed * Time.fixedDeltaTime;

        if (CheckArea(pos, halfScale))
            CrrectInArea(ref pos, halfScale);

        transform.position = pos;
    }

    private bool CheckArea(Vector3 pos, Vector3 padding)
    {
        return
            pos.x < _playArea.min.x + padding.x ||
            pos.y < _playArea.min.y + padding.y ||
            pos.x > _playArea.max.x - padding.x ||
            pos.y > _playArea.max.y - padding.y;
    }

    private void CrrectInArea(ref Vector3 pos, Vector3 padding)
    {
        if (pos.x < _playArea.min.x + padding.x)
        {
            pos.x = _playArea.min.x + padding.x;
            _moveDir.x *= -1;
        }
        if (pos.y < _playArea.min.y + padding.y)
        {
            pos.y = _playArea.min.y + padding.y;
            _moveDir.y *= -1;
        }
        if (pos.x > _playArea.max.x - padding.x)
        {
            pos.x = _playArea.max.x - padding.x;
            _moveDir.x *= -1;
        }
        if (pos.y > _playArea.max.y - padding.y)
        {
            pos.y = _playArea.max.y - padding.y;
            _moveDir.y *= -1;
        }
    }
}
