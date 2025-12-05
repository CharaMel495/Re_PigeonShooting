using UnityEngine;

[StandAloneObject]
public class CookieMachine : BossBase
{
    [System.Serializable]
    public struct Parameter
    {
        public int Life;
        public int Score;
        public float ItemProbability;
        public float MoveSpeed;
        public float ActionInterval;
        public float TurnRate;
        public int TouchDamage;
        public BulletStructs.SpreadEightShoot BulletData;
    }
    private Parameter _param;

    [SerializeField]
    private CookieMachineParameter _paramAsset;

    private BulletStructs.IBulletCreateData _bulletCreateData;
    private BulletShooter _shooter;

    public override void Initialize()
    {
        // コライダーのセットアップ
        Collider = new SelfMade.Rectangle(this.transform);
        Collider.ActorName = "CookieMachine";
        Collider.Owner = this;
        BossName = "試作型クッキー戦艦";
        Collider.ColCategory = ColliderCategory.EnemyBody;
        ColliderManager.Instance.AddCollider(Collider);

        // 便利系クラスのセットアップ
        _timer = new();
        _timer.Initialize();
        _renderer.Initialize();
        _lifeImage.Initialize();

        // パラメータを取得
        _param = _paramAsset.Parameter;
        UpdateLifeUI(1.0f);

        Life = _param.Life;

        // 弾発射周りのクラスをセットアップ
        _shooter = BulletManager.Instance.Shooter;
        _bulletCreateData = _param.BulletData;

        // アクションは指示あるまで待機
        IsActionable = false;
        IsMovable = false;

        // 移動データの構築
        // 移動は外部から行う
        MoveData = new EnemyDataStructs.TrackPlayer
        {
            Acceleration = 0.0f,
            MoveSpeed = _param.MoveSpeed,
            TurnRate = _param.TurnRate,
            Target = PlayerManager.Instance.Player
        };

        // 接触時にダメージを飛ばせるように
        TriggerEnterEventData = new DamageEventData { Damage = _param.TouchDamage };

        // イベントをバインド
        EventDispatcher.Instance.Bind(this, Collider.ActorName);
        EventDispatcher.Instance.Subscribe("StartBossBattle", StartAction);
    }

    private void FixedUpdate()
    {
        if (IsActionable)
            Action();

        _timer.Update();
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
        _timerTaskID = _timer.CreateTask(() => IsActionable = true, _param.ActionInterval);

        _bulletCreateData.Dir = new Vector3(Mathf.Cos(Time.fixedTime * Mathf.Cos(Time.fixedTime)), Mathf.Sin(Time.fixedTime * Mathf.Cos(Time.fixedTime)), 0.0f);
        _bulletCreateData.Origin = this.transform.position;
        _shooter.Shoot(_bulletCreateData);

        IsActionable = false;
    }

    [CallableEvent("OnTriggerEnter")]
    public override void OnHit(object data)
    {
        if (data is DamageEventData damgeData)
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
