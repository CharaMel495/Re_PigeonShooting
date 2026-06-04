using UnityEngine;

public class AirBaster : MonoBehaviour, IColliderbleObject
{
    [SerializeField]
    private SpriteRendererWrapper _renderer;

    private SelfMade.Circle _circle;

    public ICollider Collider => _circle;

    private DamageEventData _eventData;

    public object TriggerEnterEventData
    {
        get
        {
            _eventData.Damage = 100;
            return _eventData;
        }
    }

    public bool IsDestroyWaiting
    { get; private set; }

    public object TriggerStayEventData
    => null;

    public object TriggerExitEventData => null;

    public void Initialize(string bombOwnerName)
    {
        _circle = new SelfMade.Circle(this.transform)
        {
            ActorName = $"{bombOwnerName}Bomb",
            ColCategory = ColliderCategory.PlayerBomb,
            Owner = this
        };

        _eventData = new DamageEventData
        {
            Damage = 100
        };

        _renderer.Initialize();
    }

    public void EnActive()
    {
        _renderer.SetEnabled(true);
        ColliderManager.Instance.AddCollider(this._circle);
    }

    public void DisActive()
    {
        _renderer.SetEnabled(false);
        ColliderManager.Instance.RemoveCollider(this._circle);
    }
}
