using UnityEngine;
using BulletStructs;

/// <summary>
/// レーザー弾は挙動が特殊なのでクラスを分ける
/// </summary>
[StandAloneObject]
public class Lazer : MonoBehaviour, IColliderbleObject
{
    [SerializeField]
    private SpriteRendererWrapper _renderer;

    public ColliderCategory _colCategory
    { get; set; }

    public ICollider Collider 
        => _rect;

    public bool IsDestroyWaiting
    { get; set; }

    public object TriggerEnterEventData
        => new DamageEventData { Damage = _damage };

    public object TriggerStayEventData 
        => null;

    public object TriggerExitEventData 
        => null;

    private LazerParam _param;

    private Timer _timer;

    private Durator _durator;

    private SelfMade.Rectangle _rect;

    private int _damage;

    public void Initialize(Sprite sprite, LazerParam param)
    {
        _renderer.Initialize();
        _timer = new();
        _durator = new();
        _param = param;
        SetPosition();
        _param.Dir = (_param.Target - this.transform.position).normalized;
        float angle = Mathf.Atan2(_param.Dir.y, _param.Dir.x) * Mathf.Rad2Deg; // ラジアン→度
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        _damage = param.Damage;
        _renderer.SetSprite(SpriteManager.GetSprite(_param.SpriteType));
    }

    private void FixedUpdate()
    {
        _timer.Update();
        _durator.Update();

        Move();
    }

    private void Move()
    {
        var pos = this.transform.position;
        pos += _param.Dir * _param.MoveSpeed * Time.fixedDeltaTime;
        this.transform.position = pos;
    }

    public void SetPosition()
    {
        var currentPos = this.transform.position;
        this.transform.position = new(
            (currentPos.x + _param.Target.x) * 0.5f,
            (currentPos.y + _param.Target.y) * 0.5f);

        var length = Vector3.Distance(currentPos, _param.Target);
        var scale = this.transform.localScale;
        scale.x = length;
        this.transform.localScale = scale;
    }

    public void Shoot(float waitingTime, float openTime)
        => _timer.CreateTask(StartLazer, waitingTime);

    /// <summary>
    /// レーザーを照射開始するメソッド
    /// </summary>
    public void StartLazer()
    {
        // 判定用の矩形を登録
        _rect = (SelfMade.Rectangle)ColliderManager.Instance.CreateCollider(this.transform, ColliderType.Rectangle);
        _rect.Owner = this;
        _rect.ActorName = "PlayerBomb";
        _rect.ColCategory = _param.ColCategory;
        ColliderManager.Instance.AddCollider(_rect);

        // 一定時間をかけてレーザーが太くなるように
        // 最大まで太くなったら、指定秒数待ってから
        _durator.CreateTask(AdvanceLazer, 
            () => _timer.CreateTask(CloseLazer, _param.KeepTime), _param.OpenTime);
    }

    /// <summary>
    /// レーザーを太くしていくメソッド
    /// </summary>
    public void AdvanceLazer(float elapsedTime, float endTime)
    {
        var width = _param.Width * (elapsedTime / endTime);
        var scale = this.transform.localScale;
        scale.y = width;
        this.transform.localScale = scale;
    }

    /// <summary>
    /// レーザーを終わらせ始めるメソッド
    /// </summary>
    public void CloseLazer()
    {
        _durator.CreateTask(DisAdvanceLazer, EndLazer, _param.CloseTime);
    }

    /// <summary>
    /// レーザーを細くしていくメソッド
    /// </summary>
    public void DisAdvanceLazer(float elapsedTime, float endTime)
    {
        var ratio = 1 - (elapsedTime / endTime);
        var width = _param.Width * (ratio);
        _renderer.SetSpriteAlpha(ratio);
        var scale = this.transform.localScale;
        scale.y = width;
        this.transform.localScale = scale;
    }

    /// <summary>
    /// レーザーを終わらせるメソッド
    /// </summary>
    private void EndLazer()
    {
        ColliderManager.Instance.RemoveCollider(_rect);
        IsDestroyWaiting = true;
    }

    public void DestroyByColliderManager()
    {
        Destroy(this.gameObject);
    }
}
