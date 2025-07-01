using UnityEngine;

public class CollisionChecker : IColliderVisitor
{
    private readonly ICollider _target;
    public bool IsColliding { get; private set; }

    public CollisionChecker(ICollider target)
    {
        _target = target;
        IsColliding = false;
    }

    public void Visit(SelfMade.Circle circle)
    {
        switch (_target)
        {
            case SelfMade.Circle c:
                IsColliding = c.HitJudge(circle);
                break;
            case SelfMade.Rectangle r:
                IsColliding = r.HitJudge(circle);
                break;
        }
    }

    public void Visit(SelfMade.Rectangle rect)
    {
        switch (_target)
        {
            case SelfMade.Circle c:
                IsColliding = rect.HitJudge(c);
                break;
            case SelfMade.Rectangle r:
                IsColliding = r.HitJudge(rect);
                break;
        }
    }
}