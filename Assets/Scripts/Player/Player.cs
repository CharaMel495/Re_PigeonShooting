using System.Collections.Generic;
using UnityEngine;

namespace PlayerBullet
{
    public enum ShootType
    {
        MultiWayShot,
        MonoStraight,
        ThreeWay,
        FiveWayAndBackMono,
        TwoWay,
        Lazer,
        Wall,
    }
}

public struct DamageEventData
{
    public int Damage { get; set; }
}

/// <summary>
/// プレイヤーの本体クラス
/// </summary>
public class Player : MonoBehaviour, ITargetProvider, IColliderbleObject
{
    [SerializeField]
    private PlayerFollowCamera _camera;

    [SerializeField]
    private SpriteRendererWrapper _renderer;

    [SerializeField]
    private Transform _hitBox;

    [SerializeField]
    private Animator _animator;
    /// <summary>
    /// アニメーション管理クラス
    /// </summary>
    public Animator Animator
        => _animator;

    [SerializeField]
    private ItemVacuumer _vacuume;

    [SerializeField]
    private AirBaster _airBaster;

    [SerializeField]
    private ImageWrapper _lifeImage;

    [SerializeField]
    private CleanerUI _cleanerUI;

    [SerializeField]
    private ParticleSystem _deadParticle;

    [SerializeField]
    private PlayerDefaultStatus _defaultStatus;

    private PlayerGrowStatus _status;

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
    /// 各レベルの弾の発射方法
    /// </summary>
    private PlayerBullet.ShootType[] _shootTypeList;

    /// <summary>
    /// 現在の弾発射インターバル
    /// </summary>
    private float _intervalTime = 0;

    /// <summary>
    /// 現在のサブ弾発射タイプ
    /// </summary>
    private PlayerBullet.ShootType _currentSubShootType;

    private bool _isInvincible = false;

    public Vector3 MoveDir
    { get; set; }

    public bool IsDash
    { get; private set; } = false;

    private readonly float _bombTime = 0.1f;

    private bool _isVacuuming = false;

    private Timer _timer;
    private Durator _durator;

    private float _slopeCondition;

    private Vector3 _shotDir;

    private readonly int _maxDustValue = 10;
    private int _dustValue;
    private readonly int _maxLife = 10;
    private int _life;
    public int Life
    { get => _life;
        private set
        {
            _life = Mathf.Min(value, _maxLife);
            UpdateLifeUI(_life / (float)_maxLife);
        }    
    }

    public bool IsDestroyWaiting
    { get; private set; }

    // パラメータ名（Animator Controller内で設定したもの）
    private readonly int _animParamX = Animator.StringToHash("X");
    private readonly int _animParamY = Animator.StringToHash("Y");
    private readonly int _animParamVacuume = Animator.StringToHash("IsVacuuming");

    // コライダー関係
    private SelfMade.Circle _circle;
    public ICollider Collider => _circle;

    public object TriggerEnterEventData
        => null;

    public object TriggerStayEventData
        => null;

    public object TriggerExitEventData
        => null;

    public void Initialize()
    {
        _status = new(_defaultStatus);

        _currentSubShootType = PlayerBullet.ShootType.Lazer;

        _timer = new();
        _durator = new();

        _renderer.Initialize();
        _renderer.SetSprite(SpriteManager.GetSprite(SpriteData.SpriteType.Player));
        _renderer.SetEnabled(true);

        _lifeImage.Initialize();
        Life = _maxLife;
        _isInvincible = false;

        // 弾を発射する為の構造体はここで作っちゃう
        CreateBulletParameter();
        CreatePlayerShotList();

        _circle = new SelfMade.Circle(_hitBox)
        {
            ActorName = "Player",
            ColCategory = ColliderCategory.PlayerBody,
            Owner = this
        };
        ColliderManager.Instance.AddCollider(_circle);

        // 自動でバインドできるものをイベント登録
        EventDispatcher.Instance.Bind(this, "Player");
        // ラムダを使ってて自動バインドできないものを登録
        // 経験値取得イベント
        // TODO:敵を倒すのではなく、経験値アイテムを集めることであがるようにしたので一旦コメント化
        //EventDispatcher.Instance.Subscribe(EventNames.GetEventName(Events.OnSmashed, "Player"), (object data) => Level += (int)data);
        // 吸引イベント
        EventDispatcher.Instance.Subscribe(
            EventNames.GetEventName(Events.OnVacuumKeyPressed, "Player"), (object _) => { Vacuume(); });
        EventDispatcher.Instance.Subscribe(
            EventNames.GetEventName(Events.OnVacuumKeyReleased, "Player"), (object _) => { EndVacuume(); });
        // ダッシュイベント
        EventDispatcher.Instance.Subscribe(
            EventNames.GetEventName(Events.OnDashKeyPressed, "Player"), (object _) => { Dash(); });
        // エアーバスター
        EventDispatcher.Instance.Subscribe(
            EventNames.GetEventName(Events.OnAirBasterKeyPressed, "Player"), (object _) => { AirBaster(); });

        _vacuume.Initialize("Player", this.transform);
        _airBaster.Initialize("Player");

        _cleanerUI.UpdataValue(0.0f);
    }

    private void FixedUpdate()
    {
        _timer.Update();
        _durator.Update();

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
        else
        {
            _animator.SetFloat(_animParamX, MoveDir.x);
            _animator.SetFloat(_animParamY, MoveDir.y);
        }
    }

    /// <summary>
    /// 弾を撃つメソッド
    /// </summary>
    /// <param name="data">弾に渡すデータ</param>
    [CallableEvent("OnShotKeyPressed")]
    public void Fire(object data)
    {
        // インターバル中は弾を撃たない
        // 吸引中も弾を撃たない
        // ダッシュ中も弾を撃たない
        if (IsInterval || _isVacuuming || IsDash)
            return;

        // インターバルをセット
        _intervalTime = Mathf.Lerp(_status.MinShotRate, _status.MaxShotRate, (_dustValue / (float)_maxDustValue));

        // 特殊弾の分岐：レーザー
        if (data is BulletStructs.LazerParam)
        {
            ShootLazer((BulletStructs.LazerParam)data);
            return;
        }

        // 弾を発射する
        Shooter.Shoot(data as BulletStructs.IBulletCreateData, _slopeCondition);
        CRISoundManager.Instance.PlaySE(SFX.PlayerShot);
    }

    public void Vacuume()
    {
        if (_isVacuuming)
            return;

        _vacuume.EnActive();
        _isVacuuming = true;
        _animator.SetBool(_animParamVacuume, _isVacuuming);
    }

    public void EndVacuume()
    {
        if (!_isVacuuming)
            return;

        _vacuume.DisActive();
        _isVacuuming = false;
        _animator.SetBool(_animParamVacuume, _isVacuuming);
    }

    public void AirBaster()
    {
        if (_dustValue < 1)
            return;

        if (_dustValue < _maxDustValue)
        {
            ShootLazer((BulletStructs.LazerParam)_bulletData[PlayerBullet.ShootType.Lazer]);
        }
        else
        {
            _camera.Shake(_bombTime * 10, 1.0f);

            _airBaster.EnActive();

            _timer.CreateTask(() => _airBaster.DisActive(), _bombTime);

            CRISoundManager.Instance.PlaySE(SFX.AirBaster);
            CRISoundManager.Instance.BombEffect(_bombTime * 100);
        }

        _dustValue = 0;
        _cleanerUI.UpdataValue(0.0f);
    }

    public void Dash()
    {
        if (IsDash || _isVacuuming)
            return;

        IsDash = true;
        ColliderManager.Instance.RemoveCollider(_circle);

        _timer.CreateTask(EndDash, _status.DashTime);
    }

    private void EndDash()
    {
        IsDash = false;
        ColliderManager.Instance.AddCollider(_circle);
    }

    public void ShootLazer(BulletStructs.LazerParam param)
    {
        // TODO: 発射向きに応じてレーザーの角度を変える仕組みをつくる
        param.Origin = this.transform.position;
        param.Target = this.transform.position + _shotDir * param.Length;
        BulletManager.Instance.CreateLazer(param);
        // インターバルをセット
        //_intervalTime = param.Interval;
    }

    public void CreatePlayerShotList()
    {
        _shootTypeList = new[]
        {
            PlayerBullet.ShootType.MultiWayShot,
            PlayerBullet.ShootType.ThreeWay,
            PlayerBullet.ShootType.MonoStraight,
            PlayerBullet.ShootType.TwoWay,
            PlayerBullet.ShootType.FiveWayAndBackMono,
        };
    }

    // 弾のパラメタを更新するメソッド
    [CallableEvent("UpdateBulletParameter")]
    public void UpdateBulletParameter(object donotUse)
    {
        BulletStructs.MultiWayShot data = (BulletStructs.MultiWayShot)_bulletData[PlayerBullet.ShootType.MultiWayShot];
        data.ShotValue = _status.ShotValue;
        _bulletData[PlayerBullet.ShootType.MultiWayShot] = data;
    }

    /// <summary>
    /// 弾を作る為のデータを作成するメソッド
    /// </summary>
    public void CreateBulletParameter()
    {
        _bulletData ??= new();

        _bulletData.Add(PlayerBullet.ShootType.MultiWayShot,
            new BulletStructs.MultiWayShot
            {
                Origin = this.transform.position,
                Scale = new(1.0f, 1.3f),
                Dir = transform.right,
                MoveSpeed = 30.0f,
                AngleSpan = 90.0f,
                ColCategory = ColliderCategory.PlayerBullet,
                ShotValue = _status.ShotValue,
                SpriteType = SpriteData.SpriteType.PlayerBullet,
            });

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
                AngleSpan = 90.0f,
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
    public object GetBulletParameter(Vector2 shotDir, float slopeCondition)
    {
        var data = _bulletData[_shootTypeList[0]];

        data.Origin = this.transform.position;
        data.Dir = shotDir;
        _shotDir = shotDir;
        _animator.SetFloat(_animParamX, shotDir.x);
        _animator.SetFloat(_animParamY, shotDir.y);

        _slopeCondition = slopeCondition;

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
    public Vector3 GetPosition()
        => this.transform.position;

    [CallableEvent("OnTriggerEnter")]
    public void OnAnyEnter(object data)
    {
        if (_isInvincible)
            return;

        switch (data)
        {
            case DamageEventData:
                GetDamage((DamageEventData)data);
                break;

            case GetItemEventData:
                GetItem((GetItemEventData)data);
                break;
        }
    }

    [CallableEvent("OnGetItem")]
    public void OnGetItem(object data)
    {
        if (data is GetItemEventData itemData)
            GetItem(itemData);
    }

    private void GetItem(GetItemEventData item)
    {
        switch (item.ItemType)
        {
            //case ItemType.Battery_Green:
            //    HealHP(item.Value);
            //    CRISoundManager.Instance.PlaySE(SFX.BatteryCharge);
            //    break;
            //case ItemType.Battery_Red:
            //    //Level += item.Value;
            //    CRISoundManager.Instance.PlaySE(SFX.BatteryCharge);
            //    break;
            case ItemType.Garbage:
                _dustValue += item.Value;
                _dustValue = Mathf.Min(_dustValue, _maxDustValue);
                _cleanerUI.UpdataValue(_dustValue / (float)_maxDustValue);
                CRISoundManager.Instance.PlaySE(SFX.TypeText);
                break;
            case ItemType.EXP:
                //_status.EXP += item.Value;
                ++_status.EXP;
                break;
        }
    }

    private void GetDamage(DamageEventData data)
    {
        if (Life <= 0)
            return;

        Life -= data.Damage;

        _camera.Shake(0.1f, 0.2f);

        _isInvincible = true;

        _timer.CreateTask(() => _isInvincible = false, _status.InvincibleTime);

        CRISoundManager.Instance.PlaySE(SFX.BulletHit);

        if (Life > 0)
            return;

        _renderer.SetEnabled(false);

        _camera.Shake(1.0f, 2.0f);

        Time.timeScale = 0.25f;

        CRISoundManager.Instance.PlaySE(SFX.BossExplode);

        Instantiate(_deadParticle, this.transform.position, Quaternion.identity);

        _durator.CreateTask(ResumeTime, () =>
        {
            Time.timeScale = 1.0f;
            _timer.CreateTask(() => EventDispatcher.Instance.Dispatch("GameOver"), 1.0f);

        }, 2.0f);
    }

    private void HealHP(int value)
        => Life += value;

    private void UpdateLifeUI(float ratio)
    {
        // 0～1で clamping
        ratio = Mathf.Clamp01(ratio);

        // ゲージの進行
        _lifeImage.SetFillAmount(ratio);

        // 色の変化（緑→黄→赤）
        Color newColor;
        if (ratio > 0.5f)
        {
            // 緑→黄（0.5～1.0）
            float t = (ratio - 0.5f) * 2f;
            newColor = Color.Lerp(Color.yellow, Color.green, t);
        }
        else
        {
            // 黄→赤（0.0～0.5）
            float t = ratio * 2f;
            newColor = Color.Lerp(Color.red, Color.yellow, t);
        }

        _lifeImage.SetImageColor(newColor);
    }

    [CallableEvent("OnBossSmashed")]
    public void WhenBossSmashed(object data)
    {
        _camera.Shake(5.0f, 2.0f);

        Time.timeScale = 0.1f;

        _durator.CreateTask(ResumeTime, () => 
        { 
            Time.timeScale = 1.0f; 
            _timer.CreateTask (() => EventDispatcher.Instance.Dispatch("GameOver"), 3.0f);
        
        }, 5.0f);
    }

    private void ResumeTime(float _elapsedTime, float _endTime)
    {
        var ratio = _elapsedTime / _endTime;

        Time.timeScale = Mathf.Lerp(0.25f, 1.0f, ratio);
    }

    public void EnterInvincible()
        => _isInvincible = true;

    public void ExitInvincible()
       => _isInvincible = false;
}
