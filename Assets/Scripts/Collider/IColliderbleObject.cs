using UnityEngine;

public interface IColliderbleObject
{
    public ICollider Collider { get; }
    public object TriggerEnterEventData { get; }
    public object TriggerStayEventData { get; }
    public object TriggerExitEventData { get; }
}
