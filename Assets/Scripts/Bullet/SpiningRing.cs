using UnityEngine;

[StandAloneObject]
public class SpiningRing : MonoBehaviour, IColliderbleObject
{
    public ICollider Collider => throw new System.NotImplementedException();

    public bool IsDestroyWaiting => throw new System.NotImplementedException();

    public object TriggerEnterEventData => throw new System.NotImplementedException();

    public object TriggerStayEventData => throw new System.NotImplementedException();

    public object TriggerExitEventData => throw new System.NotImplementedException();
}
