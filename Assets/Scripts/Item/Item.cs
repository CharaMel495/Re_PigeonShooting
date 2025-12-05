using UnityEngine;

public enum ItemType
{
    Battery_Green,
    Battery_Red,
    Garbage,
    EXP,
    None
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
    public float MoveSpeed
    { get => _moveSpeed; set => _moveSpeed = value; }

    [SerializeField]
    private float _reservedMoveSpeed;

    [SerializeField]
    private SpriteRendererWrapper _renderer;

    /// <summary>
    /// 吸引されてる対象
    /// 吸引された時に、寄れるように
    /// </summary>
    private ITargetProvider _vacuumedTarget;

    public float AddtionalPower
    { get; set; }
    private float _addtionalPowerWeight;

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

    private float _outViewTime;
    private readonly float _destroyOutOfViewTime = 5.0f;

    private bool _isFixedUpdate = false;

    private bool _isReserved = false;

    public void Initialize(ItemType type, int value = 1)
    {
        _renderer.Initialize();

        _isReserved = false;

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

        EventDispatcher.Instance.Subscribe("ReserveItem", (object data) => _isReserved = true);
        EventDispatcher.Instance.Bind(this, _circle.ActorName);

        _type = type;
        _value = value;
        _renderer.SetSprite(SpriteManager.GetSprite(CastSpriteType()));

        AddtionalPower = 0f;
        _addtionalPowerWeight = DesideWeight();

        SpriteData.SpriteType CastSpriteType()
        {
            return type switch
            {
                ItemType.Battery_Green => SpriteData.SpriteType.HealItem,
                ItemType.Battery_Red => SpriteData.SpriteType.PowItem,
                ItemType.Garbage => SpriteData.SpriteType.Dust,
                ItemType.EXP => SpriteData.SpriteType.EXP,
                _ => SpriteData.SpriteType.Dust
            };
        }

        float DesideWeight()
        {
            return type switch
            {
                ItemType.Garbage => 0.45f,
                ItemType.EXP => 1.0f,
                _ => 0
            };
        }
    }

    private void FixedUpdate()
    {
        _isFixedUpdate = true;

        Move();

        if (_renderer.IsInCamera)
            return;

        _outViewTime += Time.fixedDeltaTime;

        if (_outViewTime < _destroyOutOfViewTime)
            return;

        IsDestroyWaiting = true;
    }

    private void Move()
    {
        if (_isReserved)
            MoveToPlayer();

        var pos = this.transform.position;
        pos += Dir * _moveSpeed * Time.fixedDeltaTime * AddtionalPower;
        this.transform.position = pos;

        var addtionalPowerInverse = -AddtionalPower;
        AddtionalPower += addtionalPowerInverse * Time.fixedDeltaTime;
    }

    private void Move(Vector3 dir, float pow)
    {
        if (!_isFixedUpdate)
            return;

        AddtionalPower += pow * Time.fixedDeltaTime * _addtionalPowerWeight;
        Dir = dir;

        //var pos = this.transform.position;
        //pos += dir * pow * Time.fixedDeltaTime;
        //this.transform.position = pos;

        _isFixedUpdate = false;
    }

    [CallableEvent("OnTriggerStay")]
    public void OnAnyStay(object data)
    {
        if (data is ItemVacuumer.VacuumeEvent vacuumeData)
            Move((vacuumeData._vacuumeTarget - this.transform.position).normalized, vacuumeData._vacuumePow);
        else
            Got();
    }

    private void MoveToPlayer()
    {
        var pos = this.transform.position;
        var targetDir = PlayerManager.Instance.Player.GetPosition() - pos;
        targetDir.Normalize();
        AddtionalPower += _reservedMoveSpeed * _addtionalPowerWeight * Time.fixedDeltaTime;
        pos += targetDir * AddtionalPower * Time.fixedDeltaTime;
        this.transform.position = pos;
    }

    [CallableEvent("OnTriggerExit")]
    public void OnAnyExit(object data)
    {
        AddtionalPower = 0.0f;
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
