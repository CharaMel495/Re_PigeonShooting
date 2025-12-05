using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShuBoss : BossBase
{
    [System.Serializable]
    public struct Parameter
    {
        public int Life;
        public int Score;
        public int TouchDamage;
        public float ItemProbability;
        // 突進攻撃
        public float AssaultSpeed;
        public float AssaultTurnRate;
        public float EightSpreadInterval;
        public BulletStructs.SpreadEightShoot SpreadEightBullet;
        public float EightSpreadTurnRate;
        public float AssaultTime;
        // へにょり弾
        public BulletStructs.StraightAndCurve StraightAndCurve;
        public BulletStructs.CurveAndStraight CurveAndStraight;
        public float SnakeBulletInterval;
        public float SnakeBulletTime;
        // 反射弾
        public BulletStructs.Bounce Bounce;
        public float BounveBulletTurnRate;
        public float BounceBulletInterval;
        public float BounceBulletTime;
        // 4方向自機狙い＋十字架弾
        public BulletStructs.FourWayShoot FourWayShot;
        public BulletStructs.CrossLazerParam CrossLazer;
        public float FourWayInterval;
        public float CrossLazerInterval;
        public float CrossLazerWithFourSpreadTime;
        // この葉隠れ
        public BulletStructs.MultiCurve CurveShot;
        public float CurveShotInterval;
        public float CurveShotTime;
        public float CurveShotTurnRate;
        // 発狂
    }
    private Parameter _param;

    private enum State
    {
        Assault,
        SnakeBullet,
        BounceBullet,
        CrossLazerWithFourSpread,
        CurveShot,
        OverLoad
    }
    private State _currentState;
    private Dictionary<State, Action> _stateMachine;

    [SerializeField]
    private ShuBossParameter _paramAsset;
    private BulletShooter _shooter;

    [SerializeField]
    private Animator _animator;
    private int _changeAnimTriggerHash = Animator.StringToHash("Change");

    private float _counter;
    private bool _isCrossLazer = true;
    private EnemyDataStructs.TrackPlayer _moveDataCache;
    private Vector3 _homePostion;

    public override void Initialize()
    {
        // コライダーのセットアップ
        Collider = new SelfMade.Rectangle(this.transform);
        Collider.ActorName = "ShuBoss";
        Collider.Owner = this;
        BossName = "しゅーちゃん";
        Collider.ColCategory = ColliderCategory.EnemyBody;
        ColliderManager.Instance.AddCollider(Collider);

        // 便利系クラスのセットアップ
        _timer = new();
        _timer.Initialize();
        _renderer.Initialize();
        _lifeImage.Initialize();

        // パラメータを取得
        _param = _paramAsset.Parameter;

        Life = _param.Life;
        UpdateLifeUI(1.0f);

        // 弾発射周りのクラスをセットアップ
        _shooter = BulletManager.Instance.Shooter;

        // アクションは指示あるまで待機
        IsActionable = false;
        IsMovable = false;

        // 移動は制限
        MoveData = new EnemyDataStructs.NoMove { };

        // 行動ステートを初期化
        _currentState = State.Assault;
        _counter = 0.0f;
        _stateMachine = new Dictionary<State, Action>
        {
            { State.Assault, Assault },
            { State.SnakeBullet, SnakeBulletShoot },
            { State.BounceBullet, BounceBulletShoot },
            { State.CrossLazerWithFourSpread, CrossLazerWithFourSpread },
            { State.CurveShot, CurveShot },
            { State.OverLoad, Overload },
        };

        // 接触時にダメージを飛ばせるように
        TriggerEnterEventData = new DamageEventData { Damage = _param.TouchDamage };

        // イベントをバインド
        EventDispatcher.Instance.Bind(this, Collider.ActorName);
        EventDispatcher.Instance.Subscribe("StartBossBattle", StartAction);
    }

    public override void StartAction(object data)
    {
        base.StartAction(data);

        IsMovable = false;
        _timer.CreateTask(() => IsMovable = true, 1.0f); 

        _counter = 0.0f;

        _moveDataCache = new EnemyDataStructs.TrackPlayer
        {
            MoveSpeed = _param.AssaultSpeed,
            TurnRate = _param.AssaultTurnRate,
            Target = PlayerManager.Instance.Player,
            MoveDir = PlayerManager.Instance.Player.GetPosition() - this.transform.position
        };

        MoveData = _moveDataCache;
        _homePostion = this.transform.position;
    }

    private void FixedUpdate()
    {
        if (IsActionable)
            Action();

        _timer.Update();

        _counter += Time.fixedDeltaTime;
    }

    public override void Smashed()
    {
        _timer.CanncellTask(_timerTaskID);

        EventDispatcher.Instance.Unbind(this, Collider.ActorName);
        EventDispatcher.Instance.Dispatch("BossSmashed");
        EventDispatcher.Instance.Dispatch("AddScore", _param.Score);
    }

    public override void Action()
    {
        _stateMachine[_currentState].Invoke();

        IsActionable = false;
    }

    private void Assault()
    {
        var dir = new Vector3(Mathf.Cos(Time.fixedTime * _param.EightSpreadTurnRate), Mathf.Sin(Time.fixedTime * _param.EightSpreadTurnRate));
        _param.SpreadEightBullet.Dir = dir.normalized;
        _param.SpreadEightBullet.Origin = this.transform.position;
        _shooter.Shoot(_param.SpreadEightBullet);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.EightSpreadInterval);

        if (_counter < _param.AssaultTime)
            return;

        _currentState = State.SnakeBullet;
        _counter = 0.0f;
        MoveData = new EnemyDataStructs.NoMove();
        this.transform.DOMove(_homePostion, 1.0f);

        _animator.SetTrigger(_changeAnimTriggerHash);
    }

    private void SnakeBulletShoot()
    {
        var dir = RandomVector3();
        BulletStructs.IBulletCreateData bulletData = UnityEngine.Random.value > 0.5f ? _param.StraightAndCurve : _param.CurveAndStraight;
        bulletData.Dir = dir.normalized;
        bulletData.Origin = this.transform.position;
        _shooter.Shoot(bulletData);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.SnakeBulletInterval);

        if (_counter < _param.SnakeBulletTime)
            return;

        _currentState = State.BounceBullet;
        _counter = 0.0f;

        Vector3 RandomVector3()
        {
            Vector3 v = Vector3.zero;
            v.x = UnityEngine.Random.Range(-1.0f, 1.0f);
            v.y = UnityEngine.Random.Range(-1.0f, 1.0f);
            v.z = 0;
            return v.normalized;
        }
    }

    private void BounceBulletShoot()
    {
        var dir = new Vector3(Mathf.Cos(Time.fixedTime * _param.BounveBulletTurnRate), Mathf.Sin(Time.fixedTime * _param.BounveBulletTurnRate));
        _param.Bounce.Dir = dir.normalized;
        _param.Bounce.Origin = this.transform.position;
        _shooter.Shoot(_param.Bounce);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.BounceBulletInterval);

        if (_counter < _param.BounceBulletTime)
            return;

        _currentState = State.CrossLazerWithFourSpread;
        _counter = 0.0f;
        _isCrossLazer = false;
        _timer.CreateTask(() => _isCrossLazer = true, _param.CrossLazerInterval);
    }

    private void CrossLazerWithFourSpread()
    {
        var dir = PlayerManager.Instance.Player.GetPosition() - this.transform.position;
        _param.FourWayShot.Dir = dir.normalized;
        _param.FourWayShot.Origin = this.transform.position;
        _shooter.Shoot(_param.FourWayShot);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.FourWayInterval);

        if (_isCrossLazer)
        {
            _param.CrossLazer.Dir = dir.normalized;
            _param.CrossLazer.Origin = this.transform.position;
            BulletManager.Instance.CreateCrossLazer(_param.CrossLazer);

            _isCrossLazer = false;
            _timer.CreateTask(() => _isCrossLazer = true, _param.CrossLazerInterval);
        }

        if (_counter < _param.CrossLazerWithFourSpreadTime)
            return;

        _currentState = State.CurveShot;
        _counter = 0.0f;
    }

    private void CurveShot()
    {
        var dir = new Vector3(Mathf.Cos(Time.fixedTime * _param.CurveShotTurnRate), Mathf.Sin(Time.fixedTime * _param.CurveShotTurnRate));
        _param.CurveShot.Dir = dir.normalized;
        _param.CurveShot.Origin = this.transform.position;
        _shooter.Shoot(_param.CurveShot);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.CurveShotInterval);

        if (_counter < _param.CurveShotTime)
            return;

        _currentState = State.Assault;
        _counter = 0.0f;
        _moveDataCache.MoveDir = PlayerManager.Instance.Player.GetPosition() - this.transform.position;
        MoveData = _moveDataCache;

        _animator.SetTrigger(_changeAnimTriggerHash);
    }

    private void Overload()
    {

    }

    [CallableEvent("OnTriggerEnter")]
    public override void OnHit(object data)
    {
        if (data is DamageEventData damgeData && Life > 0)
        {
            Life -= damgeData.Damage;

            UpdateLifeUI(Life / (float)_param.Life);

            if (UnityEngine.Random.value < _param.ItemProbability)
                ThrowItem(UnityEngine.Random.value < 0.5 ? ItemType.EXP : ItemType.Garbage);

            if (Life < 1)
            {
                Smashed();
            }
        }
    }
}
