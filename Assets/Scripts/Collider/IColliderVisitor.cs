using UnityEngine;

public interface IColliderVisitor
{
    void Visit(SelfMade.Rectangle rect);
    void Visit(SelfMade.Circle circle);
}