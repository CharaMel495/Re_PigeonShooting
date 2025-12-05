using BulletStructs;
using System.Collections.Generic;
using UnityEngine;

[StandAloneObject]
public class CrossLazer : MonoBehaviour, IColliderbleObject
{
    [SerializeField]
    private Transform[] _childLazers;

    public ColliderCategory _colCategory
    { get; set; }

    public ICollider Collider
        => _circle;

    public bool IsDestroyWaiting
    { get; set; }

    public object TriggerEnterEventData
        => new DamageEventData { Damage = _damage };

    public object TriggerStayEventData
        => null;

    public object TriggerExitEventData
        => null;

    private CrossLazerParam _param;

    private float _moveSpeed;

    private Timer _timer;

    private Durator _durator;

    private SelfMade.Circle _circle;
    private List<SelfMade.Rectangle> _rects;

    private int _damage;

    private Vector3 _dir;

    /// <summary>
    /// 画面外の生存時間は5秒(300f)
    /// </summary>
    private const float _LIFETIME = 10.0f;

    /// <summary>
    /// 残り生存期間
    /// </summary>
    public float RemainLifeTime
    { get; private set; }

    public bool IsInCamera
    {
        get
        {
            var screenPos = Camera.main.WorldToScreenPoint(this.transform.position);

            return !(screenPos.x < 0 || screenPos.x > Screen.width ||
                screenPos.y < 0 || screenPos.y > Screen.height);
        }
    }

    public void Initialize(CrossLazerParam param)
    {
        _timer = new();
        _durator = new();
        _param = param;
        float angle = Mathf.Atan2(_param.Dir.y, _param.Dir.x) * Mathf.Rad2Deg; // ラジアン→度
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        _damage = param.Damage;
        RemainLifeTime = _LIFETIME;
        this.transform.localScale = _param.Scale;

        _circle = new(this.transform);
        _circle.ColCategory = _param.ColCategory;
        _circle.Owner = this;
        ColliderManager.Instance.AddCollider(_circle);

        _dir = _param.Dir;

        _rects = new();

        _moveSpeed = _param.MoveSpeed;

        for (int i = 0; i < _childLazers.Length; ++i)
        {
            SelfMade.Rectangle rect = new(_childLazers[i].transform);
            rect.ColCategory = _param.ColCategory;
            rect.Owner = this;
            _rects.Add(rect);
            ColliderManager.Instance.AddCollider(rect);
        }
    }

    private void FixedUpdate()
    {
        _timer.Update();
        _durator.Update();

        Move();

        if (IsInCamera)
            return;

        RemainLifeTime -= Time.fixedDeltaTime;

        if (RemainLifeTime < 0)
            IsDestroyWaiting = true;
    }

    private void Move()
    {
        _moveSpeed += _param.Acceleration * Time.fixedDeltaTime;

        var pos = this.transform.position;
        pos += _dir * _moveSpeed * Time.fixedDeltaTime;
        pos.z = 0;
        this.transform.position = pos;

        var euler = this.transform.eulerAngles;
        euler.z += _param.AngleSpeed * Time.fixedDeltaTime;
        this.transform.eulerAngles = euler;
    }

    public void DestroyByColliderManager()
    {
        ColliderManager.Instance.RemoveCollider(_circle);

        foreach (var rect in _rects)
            ColliderManager.Instance.RemoveCollider(rect);

        Destroy(this.gameObject);
    }
}
