using UnityEngine;

namespace SelfMade
{
    public partial class Rectangle : ICollider
    {
        public void Accept(IColliderVisitor visitor) => visitor.Visit(this);
        Vector3 ICollider.Position => _transform.position;
        public ColliderCategory ColCategory { get; set; }
        public string ActorName { get; set; }
        public float GetBoundingRadius()
        {
            float width = this.LocalScale_X;
            float height = this.LocalScale_Y;
            return 0.5f * Mathf.Sqrt(width * width + height * height);
        }
    }
}
