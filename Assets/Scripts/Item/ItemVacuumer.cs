using UnityEngine;

public class ItemVacuumer : MonoBehaviour, IColliderbleObject
{
    public struct VacuumeEvent
    {
        public float _vacuumePow;
        public Vector3 _vacuumeTarget;
    }

    [SerializeField]
    private SpriteRendererWrapper _renderer;

    private Transform _ownerTransform;

    private SelfMade.Circle _circle;

    public ICollider Collider => _circle;

    private VacuumeEvent _eventData;

    public object TriggerEnterEventData => null;

    public bool IsDestroyWaiting
    { get; private set; }

    public object TriggerStayEventData
    {
        get
        {
            _eventData._vacuumeTarget = _ownerTransform.position;
            return _eventData;
        }
    }

    public object TriggerExitEventData => null;

    public void Initialize(string vacuumeOwnerName, Transform ownerTransform)
    {
        _circle = new SelfMade.Circle(this.transform)
        {
            ActorName = $"{vacuumeOwnerName}Vacuum",
            ColCategory = ColliderCategory.ItemVacuumer,
            Owner = this
        };

        _ownerTransform = ownerTransform;

        _eventData = new VacuumeEvent
        {
            _vacuumePow = 0.4f,
            _vacuumeTarget = _ownerTransform.position
        };

        _renderer.Initialize();

        EventDispatcher.Instance.Bind(this, _circle.ActorName);
    }

    private void FixedUpdate()
    {
        var euler = this.transform.eulerAngles;
        euler.z -= Time.fixedDeltaTime * 1080.0f;
        this.transform.eulerAngles = euler;
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
