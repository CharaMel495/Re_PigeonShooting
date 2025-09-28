using System;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionEventType
{
    Enter,
    Stay,
    Exit
}

public enum ColliderType
{
    Rectangle,
    Circle
}

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

public class ColliderManager : SingletonMonoBehaviour<ColliderManager>
{
    private const float CellSize = 2.0f;

    private Dictionary<Vector2Int, List<ICollider>> _spatialMap = new();
    private List<ICollider> _colliders = new();

    private Dictionary<ColliderCategory, HashSet<ColliderCategory>> _collisionPairs = new();
    private Dictionary<int, ICollider> _colliderLookup = new();

    private HashSet<CollisionPair> _previousCollisions = new();
    private HashSet<CollisionPair> _currentCollisions = new();

    private CollisionChecker _sharedChecker = new(null);

    private int _colliderID = 0;

    public void Initialize()
    {
        _spatialMap.Clear();
        _colliders.Clear();
        _collisionPairs.Clear();
        _colliderLookup.Clear();
        _previousCollisions.Clear();
        _currentCollisions.Clear();
        _colliderID = 0;

        DefineCollision(ColliderCategory.PlayerBody, ColliderCategory.EnemyBody);
        DefineCollision(ColliderCategory.PlayerBody, ColliderCategory.EnemyBullet);
        DefineCollision(ColliderCategory.EnemyBody, ColliderCategory.PlayerBullet);
        DefineCollision(ColliderCategory.EnemyBody, ColliderCategory.PlayerBomb);
        DefineCollision(ColliderCategory.EnemyBullet, ColliderCategory.PlayerBomb);
        DefineCollision(ColliderCategory.Item, ColliderCategory.PlayerBody);
        DefineCollision(ColliderCategory.Item, ColliderCategory.ItemVacuumer);
    }

    private void Update()
    {
        CheckCollisions();
    }

    public void AddCollider(ICollider collider)
    {
        collider.ID = _colliderID++;
        _colliders.Add(collider);
        _colliderLookup[collider.ID] = collider;
    }

    public void RemoveCollider(ICollider collider)
    {
        _colliders.RemoveAll(c => c.ID == collider.ID);
        _colliderLookup.Remove(collider.ID);
    }

    public void DefineCollision(ColliderCategory a, ColliderCategory b)
    {
        if (!_collisionPairs.ContainsKey(a)) _collisionPairs[a] = new();
        if (!_collisionPairs.ContainsKey(b)) _collisionPairs[b] = new();
        _collisionPairs[a].Add(b);
        _collisionPairs[b].Add(a);
    }

    public void CheckCollisions()
    {
        _spatialMap.Clear();
        _currentCollisions.Clear();

        foreach (var col in _colliders)
        {
            Vector2 center = col.Position;
            float radius = col.GetBoundingRadius();

            int minX = Mathf.FloorToInt((center.x - radius) / CellSize);
            int maxX = Mathf.FloorToInt((center.x + radius) / CellSize);
            int minY = Mathf.FloorToInt((center.y - radius) / CellSize);
            int maxY = Mathf.FloorToInt((center.y + radius) / CellSize);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (!_spatialMap.ContainsKey(cell))
                        _spatialMap[cell] = new();
                    _spatialMap[cell].Add(col);
                }
            }
        }

        foreach (var cell in _spatialMap)
        {
            List<ICollider> local = cell.Value;

            for (int i = 0; i < local.Count; ++i)
            {
                var a = local[i];

                for (int j = i + 1; j < local.Count; ++j)
                {
                    var b = local[j];

                    if (!_collisionPairs.TryGetValue(a.ColCategory, out var valid) || !valid.Contains(b.ColCategory))
                        continue;

                    _sharedChecker.Reset(a);
                    b.Accept(_sharedChecker);

                    if (_sharedChecker.IsColliding)
                    {
                        var pair = new CollisionPair(a.ID, b.ID);
                        _currentCollisions.Add(pair);

                        if (_previousCollisions.Contains(pair))
                            DispatchEvent(a, b, CollisionEventType.Stay);
                        else
                            DispatchEvent(a, b, CollisionEventType.Enter);
                    }
                }
            }
        }

        foreach (var pair in _previousCollisions)
        {
            if (_currentCollisions.Contains(pair)) continue;

            if (_colliderLookup.TryGetValue(pair.A, out var a) &&
                _colliderLookup.TryGetValue(pair.B, out var b))
            {
                DispatchEvent(a, b, CollisionEventType.Exit);
            }
        }

        (_previousCollisions, _currentCollisions) = (_currentCollisions, _previousCollisions);

        for (int i = 0; i < _colliders.Count; ++i)
        {
            var obj = _colliders[i];
            if (!obj.Owner.IsDestroyWaiting) continue;

            var type = obj.Owner.GetType();
            bool hasAttr = Attribute.IsDefined(type, typeof(StandAloneObjectAttribute));
            if (!hasAttr) continue;

            obj.Owner.DestroyByColliderManager();
            break;
        }
    }

    private Vector2Int WorldToCell(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / CellSize);
        int y = Mathf.FloorToInt(pos.y / CellSize);
        return new Vector2Int(x, y);
    }

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

            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(CastToEventName(type), a.ActorName), dataB);
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(CastToEventName(type), b.ActorName), dataA);
        }
    }

    private Events CastToEventName(CollisionEventType type) =>
        type switch
        {
            CollisionEventType.Enter => Events.OnTriggerEnter,
            CollisionEventType.Stay => Events.OnTriggerStay,
            CollisionEventType.Exit => Events.OnTriggerExit,
            _ => Events.OnGameEnd
        };

    private readonly struct CollisionPair
    {
        public readonly int A, B;
        public CollisionPair(int a, int b)
        {
            A = Mathf.Min(a, b);
            B = Mathf.Max(a, b);
        }

        public override int GetHashCode() => A * 397 ^ B;
        public override bool Equals(object obj) =>
            obj is CollisionPair other && A == other.A && B == other.B;
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
