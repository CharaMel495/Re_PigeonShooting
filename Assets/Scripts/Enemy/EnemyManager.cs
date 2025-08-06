using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    public enum EnemyID
    {
        キホンの雑魚敵,
        キホンの弾を撃つ敵,
        プレイヤーに突っ込んでくるミサイル敵,
        プレイヤーに突っ込んでくるミサイル敵_弾あり,
        ミサイル敵を撃つ敵,
        輪っか弾を撃つ敵,
        渦巻ぐるぐる敵,
        渦巻ぐるぐる敵_自機狙い単発弾,
        渦巻ぐるぐる敵_後方3way,
        バリア突進敵,
        レーザー発射敵,
        バリア突進中ボス
    }
}

/// <summary>
/// 敵管理クラス
/// </summary>
public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    [SerializeField]
    [Header("今はテスト用、将来的にはプレハブにする")]
    private Boss Hoge;

    [SerializeField]
    [Header("敵プレハブ")]
    private Enemy _enemyPrefab;

    [SerializeField]
    private Transform _stoppableArea;

    [SerializeField]
    private Transform _poolRoot;

    /// <summary>
    /// 現在アクティブな弾
    /// </summary>
    private List<Enemy> _activeEnemys;

    /// <summary>
    /// 破棄フラグが立った弾をまとめるキュー
    /// </summary>
    private Queue<Enemy> _destroyRegister;

    private EnemyAction _action;

    private EnemyPool _pool;

    private EnemyMoveParameterCreator _moveParamCreator;
    private EnemyActionParameterCreater _actionParamCreator;

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
        _pool = new(_enemyPrefab, _poolRoot);
        _moveParamCreator = new();
        _actionParamCreator = new();

        //Hoge.Initialize();
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
            if (enemy.IsInArea)
                continue;

            // 画面外の処理
            enemy.OutOfView();
        }

        // リストを更新
        FlashActiveEnemysList();
    }

    public void CreateEnemy(int tableID, Vector3 createPos, Vector3 moveDir = new())
    {
        var enemyData = StructEnemyParamFromMasterData(tableID,EnemyEnums.EnemyType.Normal);
        enemyData.Origin = createPos;

        // 敵を生成
        var enemy = _pool.GetEnemyFromPool();
        enemy.transform.localScale = enemyData.Scale;
        // 体力を注入
        enemy.Life = enemyData.Life;
        // スコアを注入
        enemy.Score = enemyData.Score;
        // 移動情報を注入
        if (moveDir != Vector3.zero)
            enemyData.MoveData.MoveDir = moveDir;
        if (enemyData.MoveData is EnemyDataStructs.StopPointMove)
        {
            var data = (EnemyDataStructs.StopPointMove)enemyData.MoveData;
            data.TargetPoint = StageManager.Instance.GetRandomPositionInArea();
            enemy.MoveData = data;
        }
        else
            enemy.MoveData = enemyData.MoveData;
        // 行動情報を注入
        enemy.ActionData = enemyData.ActionData;
        // 弾のサイズを設定
        if (enemy.ActionData.BulletData != null)
            enemy.ActionData.BulletData.Scale = enemyData.BulletSize;
        // 初期化
        enemy.EnActive(SpriteManager.GetSprite(enemyData.SpriteType), $"Enemy_{_createID}");
        // 判定用の矩形を生成
        var collider = ColliderManager.Instance.CreateCollider(enemy.transform, ColliderType.Rectangle);
        // アクタ名を登録
        collider.ActorName = enemy.Name;
        // 判定タイプを登録
        collider.ColCategory = ColliderCategory.EnemyBody;
        // オーナー登録
        collider.Owner = enemy;
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
            var enemy = _pool.GetEnemyFromPool();
            enemy.transform.localScale = enemyData.Scale;
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
            enemy.EnActive(SpriteManager.GetSprite(enemyData.SpriteType), $"Enemy_{_createID}");
            // 判定用の矩形を生成
            var collider = ColliderManager.Instance.CreateCollider(enemy.transform, ColliderType.Rectangle);
            // アクタ名を登録
            collider.ActorName = enemy.Name;
            // 判定タイプを登録
            collider.ColCategory = ColliderCategory.EnemyBody;
            // オーナー登録
            collider.Owner = enemy;
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

    public List<Enemy> CreateSpiralBarrierEnemy(int tableID, Vector3 center, int count, float radius, bool isRightSpiral, Transform parent)
    {
        float ratio = 360f / count;
        float currentTime = Time.time;

        var returnList = new List<Enemy>();

        for (int i = 0; i < count; ++i)
        {
            var enemyData = StructEnemyParamFromMasterData(tableID, EnemyEnums.EnemyType.Normal);
            enemyData.Origin = center;

            // 敵を生成
            var enemy = _pool.GetEnemyFromPool();
            var parentOffset = parent.localScale.x;
            enemy.transform.localScale = enemyData.Scale;

            // ステータス注入
            enemy.Life = enemyData.Life;
            enemy.Score = enemyData.Score;

            // 既にある MoveData を取り出して設定追加
            var moveData = (EnemyDataStructs.SlavedSpiralMove)enemyData.MoveData;

            moveData.Distance = radius;
            moveData.AddtionalTime = ratio * i * Mathf.Deg2Rad;
            moveData.IsRightSpin = isRightSpiral;

            enemy.MoveData = moveData;

            // 行動情報注入
            enemy.ActionData = enemyData.ActionData;
            if (enemy.ActionData != null)
                enemy.ActionData.BulletData.Scale = enemyData.BulletSize;

            // 初期化処理
            enemy.EnActive(SpriteManager.GetSprite(enemyData.SpriteType), $"Enemy_{_createID}");
            enemy.transform.parent = parent;

            // コライダー生成＆登録
            var collider = ColliderManager.Instance.CreateCollider(enemy.transform, ColliderType.Rectangle);
            collider.ActorName = enemy.Name;
            collider.ColCategory = ColliderCategory.EnemyBody;
            // オーナー登録
            collider.Owner = enemy;
            ColliderManager.Instance.AddCollider(collider);
            enemy.Collider = collider;

            // 管理リストへ追加
            _activeEnemys.Add(enemy);
            returnList.Add(enemy);

            ++_createID;
            if (_createID >= 100000)
                _createID = 0;
        }

        return returnList;
    }

    /// <summary>
    /// アクティブな弾の状態を更新するメソッド
    /// </summary>
    private void FlashActiveEnemysList()
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
        destroyEnemy.Destroy(_poolRoot);
        _activeEnemys.Remove(destroyEnemy);

        // メソッドの最初に飛ぶ
        goto MethodTop;
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
                    MoveData = _moveParamCreator.GetEnemyMoveData((EnemyEnums.EnemyID)data.ID),
                    ActionData = _actionParamCreator.GetEnemyActionData((EnemyEnums.EnemyID)data.ID),
                    ColliderType = (ColliderType)data.ColliderType
                };
        }

        return null;
    }
}
