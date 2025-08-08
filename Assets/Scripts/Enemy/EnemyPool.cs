using System.Linq;
using UnityEngine;

/// <summary>
/// 敵のプール
/// TODO:敵の生成しすてむをこっちにしてぷぅりんぐする
/// </summary>
public class EnemyPool
{
    /// <summary>
    /// 生成した弾をプーリングしておく変数
    /// </summary>
    private Enemy[] _enemyPool;

    /// <summary>
    /// 弾のプレハブをまとめておくオブジェクトのトランスフォーム
    /// </summary>
    private Transform _root;

    /// <summary>
    /// デフォルトコンストラクタは禁止
    /// </summary>
    private EnemyPool() { }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EnemyPool(Enemy bulletPrefab, Transform root, Rect playArea)
    {
        _enemyPool = new Enemy[300];
        _root = root;
        CreatePool(bulletPrefab, playArea);
    }

    /// <summary>
    /// 弾のプールを作成するメソッド
    /// </summary>
    public void CreatePool(Enemy enemyPrefab, Rect playArea)
    {
        for (int idx = 0; idx < _enemyPool.Length; ++idx)
        {
            // 弾を生成
            _enemyPool[idx] = Object.Instantiate(enemyPrefab, _root);
            // 弾を初期化
            _enemyPool[idx].Initialize(playArea);
        }
    }

    /// <summary>
    /// 弾プールから弾を一つ取り出すメソッド
    /// </summary>
    /// <returns>プールの内非アクティブな最初の弾を返す</returns>
    public Enemy GetEnemyFromPool()
    {
        return _enemyPool.First(bullet => !bullet.IsActive);
    }
}
