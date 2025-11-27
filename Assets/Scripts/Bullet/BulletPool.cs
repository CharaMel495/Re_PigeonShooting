using System.Linq;
using UnityEngine;

/// <summary>
/// 弾のプールを扱うクラス
/// </summary>
public class BulletPool
{
    /// <summary>
    /// 生成した弾をプーリングしておく変数
    /// </summary>
    private Bullet[] _bulletPool;

    /// <summary>
    /// 弾のプレハブをまとめておくオブジェクトのトランスフォーム
    /// </summary>
    private Transform _root;

    /// <summary>
    /// デフォルトコンストラクタは禁止
    /// </summary>
    private BulletPool() { }

    private int _nextPool;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BulletPool(Bullet bulletPrefab, Transform root, Rect playArea)
    {
        _bulletPool = new Bullet[3000];
        _root = root;
        _nextPool = -1;
        CreatePool(bulletPrefab, playArea);
    }

    /// <summary>
    /// 弾のプールを作成するメソッド
    /// </summary>
    public void CreatePool(Bullet bulletPrefab, Rect playArea)
    {
        for (int idx = 0; idx < _bulletPool.Length; ++idx)
        {
            // 弾を生成
            _bulletPool[idx] = Object.Instantiate(bulletPrefab, _root);
            // 弾を初期化
            _bulletPool[idx].Initialize(playArea);
        }
    }

    /// <summary>
    /// 弾プールから弾を一つ取り出すメソッド
    /// </summary>
    /// <returns>プールの内非アクティブな最初の弾を返す</returns>
    public Bullet GetBulletFromPool()
    {
    BEGIN:

        ++_nextPool;

        if (_nextPool >= _bulletPool.Length)
            _nextPool = 0;

        if (_bulletPool[_nextPool].IsActive)
            goto BEGIN;
        else
            return _bulletPool[_nextPool];
    }
}
