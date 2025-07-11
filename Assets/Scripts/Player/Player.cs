using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace PlayerBullet
{
    public enum ShootType
    {
        MonoStraight,
        ThreeWay,
        FourWayAndBackMono,
        TwoWay,
        Lazer,
        Wall,
    }
}

/// <summary>
/// プレイヤーの本体クラス
/// </summary>
public class Player : MonoBehaviour, ITargetProvider
{
    [SerializeField]
    private SpriteRendererWrapper _renderer;

    [SerializeField]
    private Transform _hitBox;

    public BulletShooter Shooter
    { get; set; }

    /// <summary>
    /// 弾発射のインターバルか
    /// </summary>
    public bool IsInterval
        => _intervalTime > 0;

    /// <summary>
    /// ステートマシン用の弾構造体をまとめた連想配列
    /// </summary>
    private Dictionary<PlayerBullet.ShootType, BulletStructs.IBulletCreateData> _bulletData;

    /// <summary>
    /// 弾の発射間隔
    /// </summary>
    private const float _FIREINTERVAL = 0.1f;

    /// <summary>
    /// 現在の弾発射インターバル
    /// </summary>
    private float _intervalTime = 0;

    /// <summary>
    /// 現在の弾発射タイプ
    /// </summary>
    private PlayerBullet.ShootType _currentShootType;

    /// <summary>
    /// 現在のサブ弾発射タイプ
    /// </summary>
    private PlayerBullet.ShootType _currentSubShootType;

    private const float _INVINCIBLETIME = 0.5f;

    private bool _isInvincible = false;

    private Timer _timer;

    public void Initialize()
    {
        // 弾発射イベントを登録
        EventDispatcher.Instance.Subscribe(
            EventNames.GetEventName(Events.OnShotKeyPressed, "Player"), Fire);

        EventDispatcher.Instance.Subscribe(
            EventNames.GetEventName(Events.OnSubShotKeyPressed, "Player"), Fire);

        // 最初は直進弾から
        _currentShootType = PlayerBullet.ShootType.MonoStraight;
        _currentSubShootType = PlayerBullet.ShootType.Lazer;

        _timer = new();

        _renderer.Initialize();
        _renderer.SetSprite(SpriteManager.GetSprite(SpriteData.SpriteType.Player));

        // 弾を発射する為の構造体はここで作っちゃう
        CreateBulletParameter();

        SelfMade.Circle circle = new SelfMade.Circle(_hitBox);
        circle.ActorName = "Player";
        circle.ColCategory = ColliderCategory.PlayerBody;
        ColliderManager.Instance.AddCollider(circle);

        EventDispatcher.Instance.Subscribe(EventNames.GetEventName(Events.OnHit, "Player"), OnHit);
    }

    private void FixedUpdate()
    {
        _timer.Update();

        if (_isInvincible)
        {
            if (_renderer.GetSpriteAlpha() < 0.1f)
                _renderer.SetSpriteAlpha(1.0f);
            else
                _renderer.SetSpriteAlpha(0.0f);
        }
        else if (_renderer.GetSpriteAlpha() < 0.1f)
            _renderer.SetSpriteAlpha(1.0f);

        // インターバル中ならカウントを進める
        if (IsInterval)
            _intervalTime -= Time.fixedDeltaTime;
    }

    /// <summary>
    /// 弾を撃つメソッド
    /// </summary>
    /// <param name="data">弾に渡すデータ</param>
    public void Fire(object data)
    {
        // インターバル中は弾を撃たない
        if (IsInterval)
            return;

        // インターバルをセット
        _intervalTime = _FIREINTERVAL;

        // 特殊弾の分岐：レーザー
        if (data is BulletStructs.LazerParam)
        {
            ShootLazer((BulletStructs.LazerParam)data);
            return;
        }

        // 弾を発射する
        Shooter.Shoot(data as BulletStructs.IBulletCreateData);
    }

    public void ShootLazer(BulletStructs.LazerParam param)
    {
        param.Origin = this.transform.position;
        param.Target = this.transform.position + Vector3.right * param.Length;
        BulletManager.Instance.CreateLazer(param);
        // インターバルをセット
        _intervalTime = param.Interval;
    }

    /// <summary>
    /// 弾を作る為のデータを作成するメソッド
    /// </summary>
    public void CreateBulletParameter()
    {
        _bulletData ??= new();

        _bulletData.Add(PlayerBullet.ShootType.MonoStraight, 
            new BulletStructs.StaraightShoot{
                Origin = this.transform.position,
                Scale = new (0.5f, 0.5f),
                Dir = transform.right,
                MoveSpeed = 30.0f,
                ColCategory = ColliderCategory.PlayerBullet
            });

        _bulletData.Add(PlayerBullet.ShootType.TwoWay,
            new BulletStructs.TwoWayStraightShoot
            {
                Origin = this.transform.position,
                Scale = new(0.5f, 0.5f),
                Dir = transform.right,
                MoveSpeed = 50.0f,
                BulletSpan = 0.4f,
                ColCategory = ColliderCategory.PlayerBullet
            });

        _bulletData.Add(PlayerBullet.ShootType.ThreeWay,
            new BulletStructs.ThreeWayShoot
            {
                Origin = this.transform.position,
                Scale = new(0.5f, 0.5f),
                Dir = transform.right,
                MoveSpeed = 50.0f,
                AngleSpan = 5.0f,
                ColCategory = ColliderCategory.PlayerBullet
            });

        _bulletData.Add(PlayerBullet.ShootType.Lazer,
            new BulletStructs.LazerParam
            {
                Origin = this.transform.position,
                ColCategory = ColliderCategory.PlayerBomb,
                OpenTime = 0.6f,
                CloseTime = 1.0f,
                KeepTime = 0.2f,
                SpriteType = SpriteData.SpriteType.PlayerLazer,
                Width = 0.3f,
                Length = 3.0f,
                MoveSpeed = 13.0f,
                Interval = 0.7f
            });

        _bulletData.Add(PlayerBullet.ShootType.Wall,
            new BulletStructs.LazerParam
            {
                Origin = this.transform.position,
                ColCategory = ColliderCategory.PlayerBomb,
                OpenTime = 0.4f,
                CloseTime = 1.0f,
                KeepTime = 0.2f,
                SpriteType = SpriteData.SpriteType.PlayerLazer,
                Width = 5.0f,
                Length = 0.35f,
                MoveSpeed = 3.0f,
                Interval = 2.0f
            });
    }

    /// <summary>
    /// 弾を作る為のデータを作成するメソッド
    /// </summary>
    public object GetBulletParameter()
    {
        var data = _bulletData[_currentShootType];

        data.Origin = this.transform.position;

        return data;
    }

    /// <summary>
    /// 弾を作る為のデータを作成するメソッド
    /// </summary>
    public object GetSubBulletParameter()
    {
        var data = _bulletData[_currentSubShootType];

        data.Origin = this.transform.position;

        return data;
    }

    /// <summary>
    /// インターフェースからの継承
    /// 自身の座標を公開するメソッド
    /// </summary>
    public Vector3 GetPostion()
        => this.transform.position;

    public void OnHit(object data)
    {
        if (_isInvincible)
            return;

        _isInvincible = true;

        _timer.CreateTask(() => _isInvincible = false, _INVINCIBLETIME);
    }
}
