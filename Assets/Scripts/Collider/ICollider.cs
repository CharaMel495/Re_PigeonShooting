using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// コライダーのインターフェース
/// </summary>
public interface ICollider
{
    public int ID { get; set; }
    void Accept(IColliderVisitor visitor);
    Vector3 Position { get; }
    ColliderCategory ColCategory { get; set; }
    string ActorName { get; set; }

    public bool Equals(ICollider other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ID == other.ID;
    }
}