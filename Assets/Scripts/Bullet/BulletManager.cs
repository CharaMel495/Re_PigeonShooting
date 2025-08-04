using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾管理クラス
/// </summary>
public class BulletManager : SingletonMonoBehaviour<BulletManager>
{
    [SerializeField]
    [Header("弾プレハブ")]
    private Bullet _bulletPrefab;

    [SerializeField]
    [Header("レーザー弾プレハブ")]
    private Lazer _lazerPrefab;

    [SerializeField]
    [Header("弾のプールをまとめておく場所")]
    private Transform _root;

    /// <summary>
    /// 弾のプール
    /// </summary>
    private BulletPool _pool;

    /// <summary>
    /// 現在アクティブな弾
    /// </summary>
    private List<Bullet> _activeBullets;

    /// <summary>
    /// 破棄フラグが立った弾をまとめるキュー
    /// </summary>
    private Queue<Bullet> _destroyRegister;

    /// <summary>
    /// 弾を動かすクラス
    /// </summary>
    private BulletMover _mover;

    /// <summary>
    /// 弾発射クラス
    /// 外部から参照はできるようにする
    /// </summary>
    public BulletShooter Shooter
    { get; private set; }

    private int _createID;

    public void Initialize()
    {
        _pool = new(_bulletPrefab, _root);
        _activeBullets = new();
        _destroyRegister = new();
        Shooter = new();
        _mover = new();
        _createID = 0;
    }

    private void FixedUpdate()
    {
        foreach (var bullet in _activeBullets.ToArray())
        {
            // もし破棄待ちの弾なら破棄待ちキューに突っ込んでつぎへ
            if (bullet.IsDestroyWaiting)
            {
                _destroyRegister.Enqueue(bullet);
                continue;
            }

            // 移動を行わせる
            _mover.MoveBullet(bullet);

            // 画面内に収まってたらここで終了
            if (bullet.IsInCamera)
                continue;

            // 画面外の処理
            bullet.OutOfView();
        }

        // リストを更新
        FlashActiveBulletsList();
    }

    /// <summary>
    /// 弾を生成する(ように見せかける)メソッド
    /// </summary>
    /// <param name="bulletData">生成する弾に渡すデータ</param>
    /// <param name="origin">生成した弾を作る座標</param>
    /// <param name="spriteID">画像のID</param>
    public void CreateBullet<TBulletData>(TBulletData bulletData) where TBulletData : BulletStructs.IBulletCreateData
    {
        // プールから弾を取得
        var bullet = _pool.GetBulletFromPool();
        // 移動情報を与える
        bullet.MoveData = bulletData.CreateMoveData();
        bullet.transform.right = bulletData.Dir;
        // 判定用の矩形を生成
        var collider = ColliderManager.Instance.CreateCollider(bullet.transform, ColliderType.Rectangle);
        // 判定タイプを登録
        collider.ColCategory = bulletData.ColCategory;
        // オーナー登録
        collider.Owner = bullet;
        // 判定マネージャに登録通知を飛ばす
        ColliderManager.Instance.AddCollider(collider);
        bullet.Collider = collider;
        // 座標とスケールをセット
        bullet.transform.position = bulletData.Origin;
        bullet.transform.localScale = bulletData.Scale;
        // リストに登録
        _activeBullets.Add(bullet);

        // アクティブ化
        bullet.EnActive(SpriteManager.GetSprite(bulletData.SpriteType), $"Bullet_{_createID}");

        ++_createID;

        if (_createID < 100000)
            return;

        _createID = 0;
    }

    public Lazer CreateLazer(BulletStructs.LazerParam lazerParam)
    {
        var lazer = Instantiate(_lazerPrefab, lazerParam.Origin, Quaternion.identity);
        lazer.Initialize(SpriteManager.GetSprite(lazerParam.SpriteType), lazerParam);
        lazer.StartLazer();
        return lazer;
    }

    /// <summary>
    /// アクティブな弾の状態を更新するメソッド
    /// </summary>
    private void FlashActiveBulletsList()
    {
    // 再起する代わりのラベル
    MethodTop:

        // もしキューのサイズがないのであればそも何もしない
        if (_destroyRegister.Count < 1)
            return;

        // 破棄予定の弾を１つ取得
        var destroyBullet = _destroyRegister.Dequeue();

        //弾を破棄し、リストからも削除する
        ColliderManager.Instance.RemoveCollider(destroyBullet.Collider);
        destroyBullet.Destroy(_root);
        _activeBullets.Remove(destroyBullet);

        // メソッドの最初に飛ぶ
        goto MethodTop;
    }
}
