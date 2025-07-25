using UnityEngine;
using EnemyEnums;

/// <summary>
/// 敵の本体クラス
/// (ただし、実体的にはデータ指向設計の為、概ねデータクラスである)
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField]
    [Header("画像描画するやつ")]
    private SpriteRendererWrapper _renderer;

    /// <summary>
    /// プレイエリア内に入ってるか
    /// </summary>
    public bool IsInArea
        => StageManager.Instance.PlayArea.Contains(this.transform.position);

    /// <summary>
    /// 移動情報
    /// </summary>
    public EnemyDataStructs.IEnemyMoveData MoveData
    { get; set; }

    /// <summary>
    /// 行動情報
    /// </summary>
    public EnemyDataStructs.IEnemyActionData ActionData
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

    public ICollider Collider
    { get; set; }

    private const float _INVINCIBLETIME = 0.1f;

    private bool _isInvincible = false;

    private Timer _timer;

    public int Life
    { get; set; }

    public int Score
    { get; set; }

    public void Initialize()
    {
        _renderer.Initialize();
        _renderer.SetEnabled(false);
        _timer = new();
        _timer.Initialize();
        
    }

    public void EnActive(Sprite sprite, string name)
    {
        if (_renderer.CurrentSprite != sprite)
            _renderer.SetSprite(sprite);
        _renderer.SetEnabled(true);
        this.transform.parent = null;
        RemainLifeTime = _LIFETIME;
        IsActive = true;
        IsDestroyWaiting = false;
        Name = name;

        EventDispatcher.Instance.Subscribe(EventNames.GetEventName(Events.OnHit, Name), OnHit);

        if (MoveData is EnemyDataStructs.MissileMove)
            this.transform.up = MoveData.MoveDir;
    }

    private void FixedUpdate()
    {
        if (!IsActive)
            return;

        _timer.Update();

        if (MoveData != null)
            MoveData.ElaspedTime += Time.fixedDeltaTime;

        if (_isInvincible)
        {
            if (_renderer.GetSpriteAlpha() < 0.1f)
                _renderer.SetSpriteAlpha(1.0f);
            else
                _renderer.SetSpriteAlpha(0.0f);
        }
        else if (_renderer.GetSpriteAlpha() < 0.1f)
            _renderer.SetSpriteAlpha(1.0f);

        if (ActionData == null)
            return;

        var bulletData = ActionData.BulletData;
        bulletData.Origin = this.transform.position;
    }

    public void SetActionInterval()
        => ActionData.CurrentInterval = ActionData.ActionInterval;

    public void AdvanceInterval()
        => ActionData.CurrentInterval -= Time.fixedDeltaTime;

    public void OutOfView()
    {
        --RemainLifeTime;

        if (RemainLifeTime < 1)
            IsDestroyWaiting = true;
    }

    public void OnHit(object data)
    {
        if (_isInvincible)
            return;

        _isInvincible = true;

        --Life;

        if (Life < 1)
        {
            IsDestroyWaiting = true;
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnSmashed, "Player"), 1);
            return;
        }

        _timer.CreateTask(() => _isInvincible = false, _INVINCIBLETIME);
    }

    public void Destroy(Transform poolRoot)
    {
        _renderer.SetEnabled(false);
        this.transform.parent = poolRoot;
        RemainLifeTime = _LIFETIME;
        IsActive = false;
        IsDestroyWaiting = false;
        _timer.Initialize();
        _isInvincible = false;
        EventDispatcher.Instance.Unsubscribe(EventNames.GetEventName(Events.OnHit, Name), OnHit);
    }
}
