using System.Collections.Generic;
using UnityEngine;
using Tutorial;

/// <summary>
/// チュートリアル用のプレイヤークラス
/// </summary>
public class TutorialPlayer : MonoBehaviour, ITargetProvider, IColliderbleObject
{
    [SerializeField]
    private TutorialCamera _camera;

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
    /// 弾の発射間隔
    /// </summary>
    private const float _MINFIREINTERVAL = 0.04f;

    private const float _MAXFIREINTERVAL = 0.4f;

    /// <summary>
    /// 現在の弾発射インターバル
    /// </summary>
    private float _intervalTime = 0;

    /// <summary>
    /// 現在のサブ弾発射タイプ
    /// </summary>
    private PlayerBullet.ShootType _currentSubShootType;

    private const float _INVINCIBLETIME = 0.1f;

    private bool _isInvincible = false;

    public Vector3 MoveDir
    { get; set; }

    public bool IsDash
    { get; private set; } = false;

    private readonly float _dashTime = 0.1f;
    private readonly float _bombTime = 0.1f;

    private bool _isVacuuming = false;

    private Timer _timer;

    private int _exp;

    private int _level;

    private const int _LEVELBORDER = 10;
    private const int _MAXLEVEL = 5;

    private float _slopeCondition;

    private readonly int _maxDustValue = 10;
    private int _dustValue;
    private readonly int _maxLife = 100;
    private int _life;
    public int Life
    {
        get => _life;
        private set
        {
            _life = value;
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

    private float _shotTime;

    private bool _isAirBasterLock = true;

    private int _dashCount = 0;

    /// <summary>
    /// 経験値
    /// </summary>
    public int Exp
    {
        get => _exp;
        set
        {
            _exp = value;

            EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Shot_SmashEnemy);

            if (_exp < _LEVELBORDER)
                return;

            _exp -= _LEVELBORDER;

            if (_level < _MAXLEVEL)
            {
                // TODO あとでレベルアップの仕組みをちゃんと作り込む
                BulletStructs.MultiWayShot mul = (BulletStructs.MultiWayShot)_bulletData[PlayerBullet.ShootType.MultiWayShot];
                ++mul.ShotValue;
                _bulletData[PlayerBullet.ShootType.MultiWayShot] = mul;
            }
            else
                return;//TODO:ここにレベル上限でレベルアップ時に追加スコアの記述をする
        }
    }

    public bool IsActive
    { get; private set; } = true;

    private Dictionary<Tutorials, bool> _arrowedActions;

    public void Initialize()
    {
        _status = new(_defaultStatus);

        _exp = 0;
        _level = 0;
        
        _currentSubShootType = PlayerBullet.ShootType.Lazer;

        _timer = new();

        _renderer.Initialize();
        _renderer.SetSprite(SpriteManager.GetSprite(SpriteData.SpriteType.Player));

        _lifeImage.Initialize();
        Life = _maxLife;

        // 弾を発射する為の構造体はここで作っちゃう
        CreateBulletParameter();
        CreatePlayerShotList();

        _isAirBasterLock = true;

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
        EventDispatcher.Instance.Subscribe(EventNames.GetEventName(Events.OnSmashed, "Player"), (object data) => Exp += (int)data);
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

        WhenEndTutorial(null);
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
        if (!_arrowedActions[Tutorials.Shot])
            return;

        _shotTime += Time.fixedDeltaTime;

        if (_shotTime > 5.0f)
        {
            EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Shot);
            EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.AirBaster_Shot);
        }

        // インターバル中は弾を撃たない
        // 吸引中も弾を撃たない
        // ダッシュ中も弾を撃たない
        if (IsInterval || _isVacuuming || IsDash)
            return;

        // インターバルをセット
        //_intervalTime = Mathf.Lerp(_MINFIREINTERVAL, _MAXFIREINTERVAL, (_dustValue / (float)_maxDustValue));
        _intervalTime = _status.ShotRate;

        var lessShotProbabirity = Mathf.Lerp(0, _status.LessShotProbabirity, (_dustValue / (float)_maxDustValue));

        if (Random.Range(0, 100) > 100 - lessShotProbabirity)
            return;

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

        EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Vacuum);
    }

    public void AirBaster()
    {
        if (_dustValue < _maxDustValue || _isAirBasterLock)
            return;

        _camera.Shake(_bombTime * 10, 1.0f);

        _dustValue = 0;

        _airBaster.EnActive();

        _timer.CreateTask(() => _airBaster.DisActive(), _bombTime);

        CRISoundManager.Instance.PlaySE(SFX.AirBaster);
        CRISoundManager.Instance.BombEffect(_bombTime * 100);
        _cleanerUI.UpdataValue(0.0f);

        EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.AirBaster);
    }

    public void Dash()
    {
        if (IsDash || _isVacuuming || !_arrowedActions[Tutorials.Dash])
            return;

        IsDash = true;
        //ColliderManager.Instance.RemoveCollider(_circle);

        _timer.CreateTask(EndDash, _dashTime);

        
    }

    private void EndDash()
    {
        IsDash = false;
        ++_dashCount;

        if (_dashCount >= 3)
            EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Dash);
        //ColliderManager.Instance.AddCollider(_circle);
    }

    public void ShootLazer(BulletStructs.LazerParam param)
    {
        param.Origin = this.transform.position;
        param.Target = this.transform.position + Vector3.right * param.Length;
        BulletManager.Instance.CreateLazer(param);
        // インターバルをセット
        _intervalTime = param.Interval;
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
                ShotValue = 3,
                SpriteType = SpriteData.SpriteType.PlayerBullet,
            });

        _bulletData.Add(PlayerBullet.ShootType.MonoStraight,
            new BulletStructs.StaraightShoot
            {
                Origin = this.transform.position,
                Scale = new(0.5f, 0.5f),
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
        var data = _bulletData[_shootTypeList[_level]];

        data.Origin = this.transform.position;
        data.Dir = shotDir;
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
        EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Move_ItemGet);
        EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Vacuum_Item);

        switch (item.ItemType)
        {
            case ItemType.Battery_Green:
                HealHP(item.Value);
                CRISoundManager.Instance.PlaySE(SFX.BatteryCharge);
                break;
            case ItemType.Battery_Red:
                Exp += item.Value;
                CRISoundManager.Instance.PlaySE(SFX.BatteryCharge);
                break;
            case ItemType.Garbage:
                if (!_arrowedActions[Tutorials.AirBaster])
                    break;
                _dustValue += item.Value;
                _dustValue = Mathf.Min(_dustValue, _maxDustValue);
                _cleanerUI.UpdataValue(_dustValue / (float)_maxDustValue);
                CRISoundManager.Instance.PlaySE(SFX.TypeText);
                if (_dustValue >= _maxDustValue)
                    EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.AirBaster_GetDust);
                break;
        }
    }

    private void GetDamage(DamageEventData data)
    {
        if (_isInvincible)
            return;

        if (IsDash)
        {
            EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Dash_Invincible);
            return;
        }

        Life -= data.Damage;

        _camera.Shake(0.1f, 0.2f);

        _isInvincible = true;

        _timer.CreateTask(() => _isInvincible = false, _INVINCIBLETIME);
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

    public void EnActive(Dictionary<Tutorials, bool> arrowedActions)
    {
        _arrowedActions = arrowedActions;
        this.gameObject.SetActive(true);
        _cleanerUI.gameObject.SetActive(_arrowedActions[Tutorials.AirBaster]);
        IsActive = true;
    }

    public void DisActive()
    {
        this.gameObject.SetActive(false);
        _cleanerUI.gameObject.SetActive(false);
        IsActive = false;
    }

    [CallableEvent("EndTutorial")]
    public void WhenEndTutorial(object data)
    {
        _animator.SetFloat(_animParamX, 0.0f);
        _animator.SetFloat(_animParamY, -1.0f);

        this.transform.position = Vector3.zero;

        _shotTime = 0.0f;
        _dashCount = 0;

        MoveDir = Vector3.down;

        _isAirBasterLock = true;
    }

    [CallableEvent("IsAirBasterUnLock")]
    public void AirBasterUnLock(object data)
        => _isAirBasterLock = false;
}
