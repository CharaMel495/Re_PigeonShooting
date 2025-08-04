using UnityEngine;

public interface IColliderbleObject
{
    public ICollider Collider { get; }
    public bool IsDestroyWaiting { get; }
    public object TriggerEnterEventData { get; }
    public object TriggerStayEventData { get; }
    public object TriggerExitEventData { get; }
    public void DestroyByColliderManager() { }
}
