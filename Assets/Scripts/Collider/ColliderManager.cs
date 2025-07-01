using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 判定に使用するコライダーのタイプ
/// </summary>
public enum ColliderType
{
    Rectangle,
    Circle
}

/// <summary>
/// 判定カテゴリ
/// </summary>
public enum ColliderCategory
{
    PlayerBody,
    PlayerBullet,
    PlayerBomb,
    EnemyBody,
    EnemyBullet,
}

public class ColliderManager : SingletonMonoBehaviour<ColliderManager>
{
    private List<ICollider> _colliders = new();

    // ペア定義：どのColliderType同士で判定を取るか
    private Dictionary<ColliderCategory, HashSet<ColliderCategory>> _collisionPairs = new();

    private CollisionChecker _checker;

    private int _colliderID;

    public void Initialize()
    {
        _colliders = new();
        _collisionPairs = new();

        _colliderID = 0;

        // プレイヤーの判定 vs 敵の判定
        DefineCollision(ColliderCategory.PlayerBody, ColliderCategory.EnemyBody);
        // プレイヤーの判定 vs 敵弾の判定
        DefineCollision(ColliderCategory.PlayerBody, ColliderCategory.EnemyBullet);
        // 敵の判定 vs プレイヤー弾の判定
        DefineCollision(ColliderCategory.EnemyBody, ColliderCategory.PlayerBullet);
        // 敵の判定 vs ボムの判定
        DefineCollision(ColliderCategory.EnemyBody, ColliderCategory.PlayerBomb);
        // 敵弾の判定 vs ボムの判定
        DefineCollision(ColliderCategory.EnemyBullet, ColliderCategory.PlayerBomb);
    }

    private void Update()
    {
        CheckCollisions();
    }

    public void AddCollider(ICollider collider)
    {
        collider.ID = _colliderID;
        _colliders.Add(collider);
        ++_colliderID;
    }

    public void RemoveCollider(ICollider collider)
    {
        foreach (var collider_ in _colliders.ToArray())
        {
            if (collider_.ID == collider.ID)
            {
                _colliders.Remove(collider_);
                return;
            }
        }
    }

    /// <summary>
    /// 当たり判定を定義するメソッド
    /// </summary>
    /// <param name="typeA">対象A</param>
    /// <param name="typeB">対象B</param>
    public void DefineCollision(ColliderCategory typeA, ColliderCategory typeB)
    {
        if (!_collisionPairs.ContainsKey(typeA))
            _collisionPairs[typeA] = new HashSet<ColliderCategory>();
        _collisionPairs[typeA].Add(typeB);

        if (!_collisionPairs.ContainsKey(typeB))
            _collisionPairs[typeB] = new HashSet<ColliderCategory>();
        _collisionPairs[typeB].Add(typeA);
    }

    /// <summary>
    /// 判定を確認するメソッド
    /// </summary>
    public void CheckCollisions()
    {
        for (int i = 0; i < _colliders.Count; i++)
        {
            int count = _colliders.Count;

            for (int j = i + 1; j < _colliders.Count; j++)
            {
                var a = _colliders[i];
                var b = _colliders[j];

                // この組み合わせが有効かチェック
                if (!_collisionPairs.TryGetValue(a.ColCategory, out var validTargets))
                    continue;
                if (!validTargets.Contains(b.ColCategory))
                    continue;

                var checker = new CollisionChecker(a);
                b.Accept(checker);

                if (checker.IsColliding)
                {
                    EventDispatcher.Instance.Dispatch(
                        EventNames.GetEventName(Events.OnHit, a.ActorName));
                    EventDispatcher.Instance.Dispatch(
                       EventNames.GetEventName(Events.OnHit, b.ActorName));
                }

                if (count != _colliders.Count)
                    break;
            }

            if (count != _colliders.Count)
                break;
        }
    }

    public ICollider CreateCollider(Transform transform, ColliderType type)
    {
        return type switch
        {
            ColliderType.Rectangle => new SelfMade.Rectangle(transform),
            ColliderType.Circle => new SelfMade.Circle(transform),
            _ => null
        };
    }
}
