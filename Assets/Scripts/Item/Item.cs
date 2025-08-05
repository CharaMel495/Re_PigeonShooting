using UnityEngine;

public enum ItemType
{
    Battery_Green,
    Battery_Red,
    Garbage
}

public struct GetItemEventData
{
    public ItemType ItemType { get; set; }
    public int Value { get; set; }
}

/// <summary>
/// 敵が落とすアイテム
/// </summary>
[StandAloneObject]
public class Item : MonoBehaviour, IColliderbleObject
{
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private SpriteRendererWrapper _renderer;

    /// <summary>
    /// 吸引されてる対象
    /// 吸引された時に、寄れるように
    /// </summary>
    private ITargetProvider _vacuumedTarget;

    /// <summary>
    /// 移動方向
    /// </summary>
    public Vector3 Dir
    { get; set; }

    private SelfMade.Circle _circle;

    public ICollider Collider => _circle;

    public object TriggerEnterEventData => null;

    public object TriggerStayEventData => null;

    public object TriggerExitEventData => null;

    private static int _createID = 0;

    private ItemType _type;
    private int _value;
    public bool IsDestroyWaiting
    { get; private set; } = false;

    public void Initialize(ItemType type, int value = 1)
    {
        _renderer.Initialize();

        _circle = new SelfMade.Circle(this.transform)
        {
            ActorName = $"Item{_createID}",
            ColCategory = ColliderCategory.Item,
            Owner = this
        };
        ColliderManager.Instance.AddCollider(_circle);

        ++_createID;
        if (_createID > 1000000)
            _createID = 0;

        EventDispatcher.Instance.Bind(this, _circle.ActorName);

        _type = type;
        _value = value;
        _renderer.SetSprite(SpriteManager.GetSprite(CastSpriteType()));

        SpriteData.SpriteType CastSpriteType()
        {
            return type switch
            {
                ItemType.Battery_Green => SpriteData.SpriteType.HealItem,
                ItemType.Battery_Red => SpriteData.SpriteType.PowItem,
                ItemType.Garbage => SpriteData.SpriteType.Dust,
                _ => SpriteData.SpriteType.Dust
            };
        }
    }

    private void FixedUpdate()
    {
        Move();   
    }

    private void Move()
    {
        var pos = this.transform.position;
        pos += Dir * _moveSpeed * Time.fixedDeltaTime;
        this.transform.position = pos;
    }

    private void Move(Vector3 dir, float pow)
    {
        var pos = this.transform.position;
        pos += dir * pow * Time.fixedDeltaTime;
        this.transform.position = pos;
    }

    [CallableEvent("OnTriggerStay")]
    public void OnAnyStay(object data)
    {
        if (data is ItemVacuumer.VacuumeEvent vacuumeData)
            Move((vacuumeData._vacuumeTarget - this.transform.position).normalized, vacuumeData._vacuumePow);
        else
            Got();
    }

    /// <summary>
    /// 取得された判定を受けるメソッド
    /// </summary>
    private void Got()
    {
        if (IsDestroyWaiting)
            return;

        EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnGetItem, "Player"), 
            new GetItemEventData
            {
                ItemType = _type,
                Value = _value
            });
        IsDestroyWaiting = true;
    }

    public void DestroyByColliderManager()
    {
        ColliderManager.Instance.RemoveCollider(_circle);
        Destroy(this.gameObject);
    }
}
