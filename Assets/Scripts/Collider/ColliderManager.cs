using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 衝突イベント種別（Enter: 接触開始、Stay: 接触継続中、Exit: 接触終了）
/// </summary>
public enum CollisionEventType
{
    Enter,
    Stay,
    Exit
}

/// <summary>
/// 使用する自作コライダーの形状タイプ
/// </summary>
public enum ColliderType
{
    Rectangle,
    Circle
}

/// <summary>
/// 判定カテゴリ（プレイヤー・敵・弾など）
/// </summary>
public enum ColliderCategory
{
    PlayerBody,
    PlayerBullet,
    PlayerBomb,
    EnemyBody,
    EnemyBullet,
    Item,
    ItemVacuumer
}

/// <summary>
/// 衝突判定を一元管理するマネージャークラス
/// </summary>
public class ColliderManager : SingletonMonoBehaviour<ColliderManager>
{
    /// <summary> 登録されたすべてのコライダー </summary>
    private List<ICollider> _colliders = new();

    /// <summary> 衝突判定するカテゴリのペア定義（例: Player vs Enemy）</summary>
    private Dictionary<ColliderCategory, HashSet<ColliderCategory>> _collisionPairs = new();

    /// <summary> コライダーに割り当てる一意なID用カウンタ </summary>
    private int _colliderID;

    /// <summary> 前フレームで衝突していたペア </summary>
    private HashSet<CollisionPair> _previousCollisions = new();

    /// <summary> 今フレームで衝突したペア </summary>
    private HashSet<CollisionPair> _currentCollisions = new();

    /// <summary>
    /// マネージャー初期化（コライダー・ペア情報の初期化）
    /// </summary>
    public void Initialize()
    {
        _colliders.Clear();
        _collisionPairs.Clear();
        _previousCollisions.Clear();
        _currentCollisions.Clear();
        _colliderID = 0;

        // 衝突対象となるカテゴリのペアを定義
        DefineCollision(ColliderCategory.PlayerBody, ColliderCategory.EnemyBody);
        DefineCollision(ColliderCategory.PlayerBody, ColliderCategory.EnemyBullet);
        DefineCollision(ColliderCategory.EnemyBody, ColliderCategory.PlayerBullet);
        DefineCollision(ColliderCategory.EnemyBody, ColliderCategory.PlayerBomb);
        DefineCollision(ColliderCategory.EnemyBullet, ColliderCategory.PlayerBomb);
        DefineCollision(ColliderCategory.Item, ColliderCategory.PlayerBody);
        DefineCollision(ColliderCategory.Item, ColliderCategory.ItemVacuumer);
    }

    /// <summary>
    /// 毎フレーム呼ばれ、コライダー同士の衝突チェックを行う
    /// </summary>
    private void Update()
    {
        CheckCollisions();
    }

    /// <summary>
    /// 新しいコライダーを登録
    /// </summary>
    public void AddCollider(ICollider collider)
    {
        collider.ID = _colliderID++;
        _colliders.Add(collider);
    }

    /// <summary>
    /// コライダーの登録解除
    /// </summary>
    public void RemoveCollider(ICollider collider)
    {
        _colliders.RemoveAll(c => c.ID == collider.ID);
    }

    /// <summary>
    /// どのカテゴリ同士が衝突判定を持つかを登録する
    /// </summary>
    public void DefineCollision(ColliderCategory typeA, ColliderCategory typeB)
    {
        if (!_collisionPairs.ContainsKey(typeA))
            _collisionPairs[typeA] = new();
        _collisionPairs[typeA].Add(typeB);

        if (!_collisionPairs.ContainsKey(typeB))
            _collisionPairs[typeB] = new();
        _collisionPairs[typeB].Add(typeA);
    }

    /// <summary>
    /// 全登録コライダー間の衝突をチェックし、イベントを発火
    /// </summary>
    public void CheckCollisions()
    {
        _currentCollisions.Clear();

        int count = _colliders.Count;

        for (int i = 0; i < count; ++i)
        {
            var a = _colliders[i];

            for (int j = i + 1; j < count; ++j)
            {
                var b = _colliders[j];

                // カテゴリの組み合わせが有効か確認
                if (!_collisionPairs.TryGetValue(a.ColCategory, out var validTargets))
                    continue;
                if (!validTargets.Contains(b.ColCategory))
                    continue;

                // 衝突判定の実行（Visitorパターン）
                var checker = new CollisionChecker(a);
                b.Accept(checker);

                if (checker.IsColliding)
                {
                    var pair = new CollisionPair(a.ID, b.ID);
                    _currentCollisions.Add(pair);

                    if (_previousCollisions.Contains(pair))
                        DispatchEvent(a, b, CollisionEventType.Stay); // 継続
                    else
                        DispatchEvent(a, b, CollisionEventType.Enter); // 新規衝突
                }
            }
        }

        // 前フレームでは衝突していたが、今フレームで解消されたもの = Exit
        foreach (var pair in _previousCollisions)
        {
            if (!_currentCollisions.Contains(pair))
            {
                var a = _colliders.Find(c => c.ID == pair.A);
                var b = _colliders.Find(c => c.ID == pair.B);
                if (a != null && b != null)
                    DispatchEvent(a, b, CollisionEventType.Exit);
            }
        }

        // 衝突状態を次フレームに持ち越し（差分抽出のため）
        (_previousCollisions, _currentCollisions) = (_currentCollisions, _previousCollisions);

        foreach (var obj in _colliders.ToArray())
        {
            if (!obj.Owner.IsDestroyWaiting)
                continue;

            var type = obj.Owner.GetType();
            bool hasAttr = Attribute.IsDefined(type, typeof(StandAloneObjectAttribute));
            if (!hasAttr)
                continue;
            // もしマネージャー無しのクラスならここで消す
            obj.Owner.DestroyByColliderManager();
        }
    }

    /// <summary>
    /// 衝突イベントを両者に対して送出
    /// </summary>
    private void DispatchEvent(ICollider a, ICollider b, CollisionEventType type)
    {
        if (a.Owner is IColliderbleObject ao && b.Owner is IColliderbleObject bo)
        {
            object dataA = type switch
            {
                CollisionEventType.Enter => ao.TriggerEnterEventData,
                CollisionEventType.Stay => ao.TriggerStayEventData,
                CollisionEventType.Exit => ao.TriggerExitEventData,
                _ => null
            };

            object dataB = type switch
            {
                CollisionEventType.Enter => bo.TriggerEnterEventData,
                CollisionEventType.Stay => bo.TriggerStayEventData,
                CollisionEventType.Exit => bo.TriggerExitEventData,
                _ => null
            };

            // EventDispatcher にてイベント名を生成・発火
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(CastToEventName(type), a.ActorName), dataB);
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(CastToEventName(type), b.ActorName), dataA);
        }
    }

    /// <summary>
    /// 自作コライダー生成用のファクトリ
    /// </summary>
    public ICollider CreateCollider(Transform transform, ColliderType type)
    {
        return type switch
        {
            ColliderType.Rectangle => new SelfMade.Rectangle(transform),
            ColliderType.Circle => new SelfMade.Circle(transform),
            _ => null
        };
    }

    /// <summary>
    /// 衝突ペア（順不同）を表す構造体
    /// </summary>
    private struct CollisionPair
    {
        public int A, B;
        public CollisionPair(int a, int b)
        {
            A = Mathf.Min(a, b); // 順不同でも同じペアとして扱う
            B = Mathf.Max(a, b);
        }

        public override bool Equals(object obj) =>
            obj is CollisionPair other && A == other.A && B == other.B;

        public override int GetHashCode() =>
            A * 397 ^ B;
    }

    private Events CastToEventName(CollisionEventType type)
    {
        return type switch
        {
            CollisionEventType.Enter => Events.OnTriggerEnter,
            CollisionEventType.Stay => Events.OnTriggerStay,
            CollisionEventType.Exit => Events.OnTriggerExit,
            _ => Events.OnGameEnd
        };
    }
}
