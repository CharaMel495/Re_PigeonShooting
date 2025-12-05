using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[StandAloneObject]
public class RollCastle : BossBase
{
    [System.Serializable]
    public struct Parameter
    {
        public int Life;
        public int Score;
        public float ItemProbability;
        public float CrossBulletInterval;
        public float CrossBuletTime;
        public float CurveShotInterval;
        public float CurveShotTime;
        public float CrossLazerInterval;
        public float CrossLazerTime;
        public float CrossBulletTurnRate;
        public float CurveShotTurnRate;
        public int TouchDamage;
        public BulletStructs.SpreadFourShoot CrossBullet;
        public BulletStructs.MultiCurve CurveShot;
        public BulletStructs.CrossLazerParam CrossLazer;
    }
    private Parameter _param;

    private enum State
    {
        十字レーザー,
        木の葉隠れ,
        十字架弾発射
    }
    private State _currentState;
    private Dictionary<State, Action> _stateMachine;

    [SerializeField]
    private RollCastleParameter _paramAsset;
    private BulletShooter _shooter;

    private float _counter;

    public override void Initialize()
    {
        // コライダーのセットアップ
        Collider = new SelfMade.Rectangle(this.transform);
        Collider.ActorName = "RollCastle";
        Collider.Owner = this;
        BossName = "ロールケーキ城";
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
        MoveData = new EnemyDataStructs.NoMove{};

        // 行動ステートを初期化
        _currentState = State.十字レーザー;
        _counter = 0.0f;
        _stateMachine = new Dictionary<State, Action>
        {
            { State.十字レーザー, ShootCrossBullet },
            { State.木の葉隠れ, ShootMultiCurveBullet },
            { State.十字架弾発射, ShootCrossLazer },
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

        _counter = 0.0f;
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

    private void ShootCrossBullet()
    {
        var dir = new Vector3(Mathf.Cos(Time.fixedTime * _param.CrossBulletTurnRate), Mathf.Sin(Time.fixedTime * _param.CrossBulletTurnRate));
        _param.CrossBullet.Dir = dir.normalized;
        _param.CrossBullet.Origin = this.transform.position;
        _shooter.Shoot(_param.CrossBullet);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.CrossBulletInterval);

        if (_counter < _param.CrossBuletTime)
            return;

        _currentState = State.木の葉隠れ;
        _counter = 0.0f;
    }

    private void ShootMultiCurveBullet()
    {
        var dir = new Vector3(Mathf.Cos(Time.fixedTime * _param.CurveShotTurnRate), Mathf.Sin(Time.fixedTime * _param.CurveShotTurnRate));
        _param.CurveShot.Dir = dir.normalized;
        _param.CurveShot.Origin = this.transform.position;
        _shooter.Shoot(_param.CurveShot);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.CurveShotInterval);

        if (_counter < _param.CurveShotTime)
            return;

        _currentState = State.十字架弾発射;
        _counter = 0.0f;
    }

    private void ShootCrossLazer()
    {
        var dir = PlayerManager.Instance.Player.GetPosition() - this.transform.position;
        _param.CrossLazer.Dir = dir.normalized;
        _param.CrossLazer.Origin = this.transform.position;
        BulletManager.Instance.CreateCrossLazer(_param.CrossLazer);

        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.CrossLazerInterval);

        if (_counter < _param.CrossLazerTime)
            return;

        _currentState = State.十字レーザー;
        _counter = 0.0f;
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
