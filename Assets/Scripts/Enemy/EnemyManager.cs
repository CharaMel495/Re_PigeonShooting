using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEngine.Rendering.DebugUI;

namespace EnemyEnums
{
    /// <summary>
    /// 敵タイプ
    /// </summary>
    public enum EnemyType
    {
        Normal,
        Boss,
    }

    /// <summary>
    /// 敵の行動タイプ
    /// </summary>
    public enum EnemyMoveType
    {
        Straight,
        StopPoint,
        Spiral_R,
        Spiral_L,
        ZigZag,
        BarrierSpiral_R,
        BarrierSpiral_L,
    }

    public enum EnemyActionType
    {
        None,
        ShootMono,
        ShootThreeWay,
        ShootFourWay,
        SpreadEight,
        InverceThreeWay,
    }
}

/// <summary>
/// 敵管理クラス
/// </summary>
public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    [SerializeField]
    [Header("敵プレハブ")]
    private Enemy _enemyPrefab;

    [SerializeField]
    private Transform _stoppableArea;

    /// <summary>
    /// 現在アクティブな弾
    /// </summary>
    private List<Enemy> _activeEnemys;

    /// <summary>
    /// 破棄フラグが立った弾をまとめるキュー
    /// </summary>
    private Queue<Enemy> _destroyRegister;

    private EnemyAction _action;

    /// <summary>
    /// 弾発射クラス
    /// 外部から参照はできるようにする
    /// </summary>
    public BulletShooter Shooter
    { get; private set; }

    /// <summary>
    /// 生成ID
    /// </summary>
    private int _createID;

    private EnemyParamTableAsset _tableAsset;

    public void Initialize()
    {
        _destroyRegister = new();
        _activeEnemys = new();
        _action = new();
        Shooter = BulletManager.Instance.Shooter;
        _tableAsset = Addressables.LoadAssetAsync<EnemyParamTableAsset>
            (SummarizeResourceDirectory.ENEMYTABLEASSET_PATH).WaitForCompletion();
        _createID = 0;
    }

    private void FixedUpdate()
    {
        foreach (var enemy in _activeEnemys.ToArray())
        {
            // もし破棄待ちの敵なら破棄待ちキューに突っ込んでつぎへ
            if (enemy.IsDestroyWaiting)
            {
                _destroyRegister.Enqueue(enemy);
                continue;
            }

            // 行動させる
            _action.Move(enemy);
            _action.Action(enemy, Shooter);

            // 画面内に収まってたらここで終了
            if (enemy.IsInCamera)
                continue;

            // 画面外の処理
            enemy.OutOfView();
        }

        // リストを更新
        FlashActiveBulletsList();
    }

    public void CreateEnemy(int tableID, Vector3 createPos)
    {
        var enemyData = StructEnemyParamFromMasterData(tableID,EnemyEnums.EnemyType.Normal);
        enemyData.Origin = createPos;

        // 敵を生成
        var enemy = Instantiate(_enemyPrefab, enemyData.Origin, Quaternion.identity);
        // 体力を注入
        enemy.Life = enemyData.Life;
        // スコアを注入
        enemy.Score = enemyData.Score;
        // 移動情報を注入
        if (enemyData.MoveData is EnemyDataStructs.StopPointMove)
        {
            var data = (EnemyDataStructs.StopPointMove)enemyData.MoveData;
            data.TargetPoint = GetRandomStopPos();
            enemy.MoveData = data;
        }
        else
            enemy.MoveData = enemyData.MoveData;
        // 行動情報を注入
        enemy.ActionData = enemyData.ActionData;
        // 弾のサイズを設定
        if (enemy.ActionData != null)
            enemy.ActionData.BulletData.Scale = enemyData.BulletSize;
        // 初期化
        enemy.Initialize(SpriteManager.GetSprite(enemyData.SpriteType), $"Enemy_{_createID}");
        // 判定用の矩形を生成
        var collider = ColliderManager.Instance.CreateCollider(enemy.transform, ColliderType.Rectangle);
        // アクタ名を登録
        collider.ActorName = enemy.Name;
        // 判定タイプを登録
        collider.ColCategory = ColliderCategory.EnemyBody;
        // 判定マネージャに登録通知を飛ばす
        ColliderManager.Instance.AddCollider(collider);
        // 生成した敵にコライダーの情報を記憶させる
        enemy.Collider = collider;
        // 管理対象として追加
        _activeEnemys.Add(enemy);

        ++_createID;

        if (_createID < 100000)
            return;

        _createID = 0;

        Vector3 GetRandomStopPos()
        {
            Vector2 min = new(
                _stoppableArea.position.x - _stoppableArea.localScale.x * 0.5f,
                _stoppableArea.position.y - _stoppableArea.localScale.y * 0.5f
                );

            Vector2 max = new(
                _stoppableArea.position.x + _stoppableArea.localScale.x * 0.5f,
                _stoppableArea.position.y + _stoppableArea.localScale.y * 0.5f
                );

            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                0.0f
                );
        }
    }

    public void CreateSpiralEnemy(Vector3 center, int count, bool isRightSpiral)
    {
        int tableID = isRightSpiral ? 9 : 10;

        var ratio = 360 / count;
        
        for (int i = 0; i < count; ++i)
        {
            var enemyData = StructEnemyParamFromMasterData(tableID, EnemyEnums.EnemyType.Normal);
            enemyData.Origin = center;
            // 敵を生成
            var enemy = Instantiate(_enemyPrefab, enemyData.Origin, Quaternion.identity);
            // 体力を注入
            enemy.Life = enemyData.Life;
            // スコアを注入
            enemy.Score = enemyData.Score;
            // 移動情報を注入
            var startAngle = ratio * i;
            enemyData.MoveData.MoveDir = Quaternion.AngleAxis(
                startAngle, Vector3.forward) * enemyData.MoveData.MoveDir;
            enemy.MoveData = enemyData.MoveData;
            // 行動情報を注入
            enemy.ActionData = enemyData.ActionData;
            // 弾のサイズを設定
            if (enemy.ActionData != null)
                enemy.ActionData.BulletData.Scale = enemyData.BulletSize;
            // 初期化
            enemy.Initialize(SpriteManager.GetSprite(enemyData.SpriteType), $"Enemy_{_createID}");
            // 判定用の矩形を生成
            var collider = ColliderManager.Instance.CreateCollider(enemy.transform, ColliderType.Rectangle);
            // アクタ名を登録
            collider.ActorName = enemy.Name;
            // 判定タイプを登録
            collider.ColCategory = ColliderCategory.EnemyBody;
            // 判定マネージャに登録通知を飛ばす
            ColliderManager.Instance.AddCollider(collider);
            // 生成した敵にコライダーの情報を記憶させる
            enemy.Collider = collider;
            // 管理対象として追加
            _activeEnemys.Add(enemy);

            ++_createID;

            if (_createID < 100000)
                continue;

            _createID = 0;
        }
    }

    public List<Enemy> CreateSpiralBarrierEnemy(Vector3 center, int count, bool isRightSpiral, Transform parent)
    {
        int tableID = isRightSpiral ? 11 : 12;

        var ratio = 360 / count;

        var returnList = new List<Enemy>();

        for (int i = 0; i < count; ++i)
        {
            var enemyData = StructEnemyParamFromMasterData(tableID, EnemyEnums.EnemyType.Normal);
            enemyData.Origin = center;
            // 敵を生成
            var enemy = Instantiate(_enemyPrefab, parent);
            // 体力を注入
            enemy.Life = enemyData.Life;
            // スコアを注入
            enemy.Score = enemyData.Score;
            // 移動情報を注入
            var startAngle = ratio * i;
            enemyData.MoveData.MoveDir = Quaternion.AngleAxis(
                startAngle, Vector3.forward) * enemyData.MoveData.MoveDir;
            enemy.MoveData = enemyData.MoveData;
            // 行動情報を注入
            enemy.ActionData = enemyData.ActionData;
            // 弾のサイズを設定
            if (enemy.ActionData != null)
                enemy.ActionData.BulletData.Scale = enemyData.BulletSize;
            // 初期化
            enemy.Initialize(SpriteManager.GetSprite(enemyData.SpriteType), $"Enemy_{_createID}");
            // 判定用の矩形を生成
            var collider = ColliderManager.Instance.CreateCollider(enemy.transform, ColliderType.Rectangle);
            // アクタ名を登録
            collider.ActorName = enemy.Name;
            // 判定タイプを登録
            collider.ColCategory = ColliderCategory.EnemyBody;
            // 判定マネージャに登録通知を飛ばす
            ColliderManager.Instance.AddCollider(collider);
            // 生成した敵にコライダーの情報を記憶させる
            enemy.Collider = collider;
            // 管理対象として追加
            _activeEnemys.Add(enemy);

            ++_createID;

            returnList.Add(enemy);

            if (_createID < 100000)
                continue;

            _createID = 0;
        }

        return returnList;
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
        var destroyEnemy = _destroyRegister.Dequeue();

        //弾を破棄し、リストからも削除する
        ColliderManager.Instance.RemoveCollider(destroyEnemy.Collider);
        Destroy(destroyEnemy.gameObject);
        _activeEnemys.Remove(destroyEnemy);

        // メソッドの最初に飛ぶ
        goto MethodTop;
    }

    public EnemyDataStructs.IEnemyMoveData GetEnemyMoveData(EnemyEnums.EnemyMoveType moveType)
    {
        switch (moveType)
        {
            case EnemyEnums.EnemyMoveType.Straight:
                return new EnemyDataStructs.StrainghtNormalMove
                {
                    Acceleration = 0.0f,
                    MoveDir = -Vector3.right,
                    MoveSpeed = 3.0f
                };

            case EnemyEnums.EnemyMoveType.StopPoint:
                return new EnemyDataStructs.StopPointMove
                {
                    MoveSpeed = 2.0f,
                    StopThreshold = 0.1f
                };

            case EnemyEnums.EnemyMoveType.Spiral_R:
                return new EnemyDataStructs.SpiralMove
                {
                    Acceleration = 0.8f,
                    MoveDir = Vector3.left,
                    MoveSpeed = 3.0f,
                    SpiralRatio = 60.0f
                };

            case EnemyEnums.EnemyMoveType.Spiral_L:
                return new EnemyDataStructs.SpiralMove
                {
                    Acceleration = 0.8f,
                    MoveDir = Vector3.right,
                    MoveSpeed = 3.0f,
                    SpiralRatio = -60.0f
                };

            case EnemyEnums.EnemyMoveType.BarrierSpiral_R:
                return new EnemyDataStructs.SpiralMove
                {
                    Acceleration = 0.0f,
                    MoveDir = Vector3.left,
                    MoveSpeed = 5.0f,
                    SpiralRatio = 180.0f
                };

            case EnemyEnums.EnemyMoveType.BarrierSpiral_L:
                return new EnemyDataStructs.SpiralMove
                {
                    Acceleration = 0.0f,
                    MoveDir = Vector3.right,
                    MoveSpeed = 5.0f,
                    SpiralRatio = -180.0f
                };
        }


        return null;
    }

    public EnemyDataStructs.IEnemyActionData GetEnemyActionData(EnemyEnums.EnemyActionType actionType)
    {
        switch (actionType)
        {
            case EnemyEnums.EnemyActionType.ShootMono:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    BulletData = new BulletStructs.StaraightShoot
                    {
                        MoveSpeed = 5.0f,
                        Scale = Vector3.one,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet
                    },
                    Target = PlayerManager.Instance.Player
                };

            case EnemyEnums.EnemyActionType.ShootThreeWay:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    BulletData = new BulletStructs.ThreeWayShoot
                    {
                        MoveSpeed = 5.0f,
                        Scale = Vector3.one,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                        AngleSpan = 30.0f
                    },
                    Target = PlayerManager.Instance.Player
                };

            case EnemyEnums.EnemyActionType.ShootFourWay:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    BulletData = new BulletStructs.FourWayShoot
                    {
                        MoveSpeed = 5.0f,
                        Scale = Vector3.one,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                        AngleSpan = 20.0f
                    },
                    Target = PlayerManager.Instance.Player
                };

            case EnemyEnums.EnemyActionType.SpreadEight:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    BulletData = new BulletStructs.SpreadEightShoot
                    {
                        MoveSpeed = 5.0f,
                        Scale = Vector3.one,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                        Dir = Vector3.right
                    },
                };

            case EnemyEnums.EnemyActionType.InverceThreeWay:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 0.2f,
                    BulletData = new BulletStructs.ThreeWayShoot
                    {
                        MoveSpeed = 5.0f,
                        Acceleration = 0.0f,
                        Scale = Vector3.one * 0.25f,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                        AngleSpan = 20.0f
                    },
                };
        }

        return null;
    }

    public EnemyDataStructs.IEnemyParam StructEnemyParamFromMasterData(int tableID, EnemyEnums.EnemyType enemyType)
    {
        if (_tableAsset.EnemyTable.Count < tableID || tableID < 0)
            return null;

        var data = _tableAsset.EnemyTable[tableID];

        switch (enemyType)
        {
            case EnemyEnums.EnemyType.Normal:
                return new EnemyDataStructs.NormalEnemyParam
                {
                    Life = data.Life,
                    Score = data.Score,
                    Scale = new Vector3(data.Size, data.Size, data.Size),
                    BulletSize = new Vector3(data.BulletSize, data.BulletSize, data.BulletSize),
                    SpriteType = (SpriteData.SpriteType)data.SpriteID,
                    MoveData = GetEnemyMoveData((EnemyEnums.EnemyMoveType)data.MoveType),
                    ActionData = GetEnemyActionData((EnemyEnums.EnemyActionType)data.ActionType),
                    ColliderType = (ColliderType)data.ColliderType
                };
        }

        return null;
    }
}
