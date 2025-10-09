using UnityEngine;

/// <summary>
/// 弾クラス。ただ、実体はデータ指向のため
/// ほとんどデータクラス
/// </summary>
public class Bullet : MonoBehaviour, IColliderbleObject
{
    [SerializeField]
    [Header("画像描画するやつ")]
    private SpriteRendererWrapper _renderer;

    private Rect _playArea;

    /// <summary>
    /// カメラに映ってるか
    /// </summary>
    public bool IsInCamera
        => _renderer.IsInCamera;
        //=> _playArea.Contains(this.transform.position);

    /// <summary>
    /// 移動情報
    /// </summary>
    public BulletStructs.IBulletMoveData MoveData
    { get; set; }

    /// <summary>
    /// 現在アクティブか
    /// </summary>
    public bool IsActive
    { get; private set; } = false;

    /// <summary>
    /// 画面外の生存時間は5秒(300f)
    /// </summary>
    private const int _LIFETIME = 30;

    /// <summary>
    /// 残り生存期間
    /// </summary>
    public int RemainLifeTime
    { get; private set; }

    /// <summary>
    /// 破棄待ちフラグ
    /// </summary>
    public bool IsDestroyWaiting
    { get; private set; }

    public string Name
    { get; private set; }

    public int Damage
    { get; set; }

    public ICollider Collider
    { get; set; }

    public object TriggerEnterEventData
        => new DamageEventData { Damage = Damage };

    public object TriggerStayEventData
        => null;

    public object TriggerExitEventData
        => null;

    public int Score
    { get; set; } = 50;

    public void Initialize(Rect playArea)
    {
        _renderer.Initialize();
        _renderer.SetEnabled(false);
        _playArea = playArea;
    }

    public void EnActive(Sprite sprite, string name)
    {
        // 異なるスプライトになる場合のみスプライトを切り替える
        if (_renderer.CurrentSprite != sprite)
            _renderer.SetSprite(sprite);
        var pos = this.transform.position;
        pos += this.transform.right * (this.transform.localScale.x * 0.5f);
        transform.position = pos;
        _renderer.SetEnabled(true);
        this.transform.parent = null;
        RemainLifeTime = _LIFETIME;
        IsActive = true;
        IsDestroyWaiting = false;
        Name = name;
        Collider.ActorName = Name;

        EventDispatcher.Instance.Bind(this, Name);

        //EventDispatcher.Instance.Subscribe(EventNames.GetEventName(Events.OnHit, Name), OnHit);
    }

    public void OutOfView()
    {
        --RemainLifeTime;

        if (RemainLifeTime < 1)
            IsDestroyWaiting = true;
    }

    public void Destroy(Transform poolRoot)
    {
        _renderer.SetEnabled(false);
        this.transform.parent = poolRoot;
        RemainLifeTime = _LIFETIME;
        IsActive = false;
        IsDestroyWaiting = false;
        EventDispatcher.Instance.Unbind(this, Name);
        this.transform.localScale = Vector3.one;
        this.transform.rotation = Quaternion.identity;
    }

    [CallableEvent("OnTriggerEnter")]
    public void OnHit(object data)
    {
        if (data is DamageEventData damageData)
        {
            if (damageData.Damage > 50)
                EventDispatcher.Instance.Dispatch("AddScore", Score);
        }

        IsDestroyWaiting = true;
    }
}