using UnityEngine;

namespace SelfMade
{
    public partial class Circle : ICollider
    {
        public void Accept(IColliderVisitor visitor) => visitor.Visit(this);
        Vector3 ICollider.Position => Position;
        public ColliderCategory ColCategory { get; set; }
        public string ActorName { get; set; }
        public float GetBoundingRadius() => Radius;
    }
}