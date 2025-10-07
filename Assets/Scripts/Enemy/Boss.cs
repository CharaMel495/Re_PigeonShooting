using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ボスの本体クラス
/// </summary>
[StandAloneObject]
public class Boss : MonoBehaviour, IColliderbleObject
{
    private enum ActionType
    {
        SpreadBarrage,
        ShootMissile,
        DiscShot
    }

    public EnemyDataStructs.TrackPlayer MoveData
    { get; set; }

    [SerializeField]
    private ParticleSystem _particle;

    /// <summary>
    /// 現在のアクション
    /// </summary>
    private ActionType _currentAction;

    private ActionType[] _actionPattern;

    private int _currentPattern = 0;

    private Dictionary<ActionType, BossDataStructs.IBossAction> _actions;

    [SerializeField]
    private SpriteRendererWrapper _renderer;

    private SelfMade.Rectangle _rect;

    private const float _INVINCIBLETIME = 0.1f;

    private bool _isInvincible = false;

    private Timer _timer;

    public int Life
    { get; set; } = 1000;

    public int Score
    { get; set; }

    private int _effectCount = 50;

    public bool IsDestroyWaiting
    { get; private set; }

    public ICollider Collider => _rect;

    public object TriggerEnterEventData
        => null;

    public object TriggerStayEventData
        => null;

    public object TriggerExitEventData
        => null;

    public void Initialize()
    {
        _renderer.Initialize();
        _renderer.SetSprite(SpriteManager.GetSprite(SpriteData.SpriteType.BossShip));

        _rect = new(this.transform)
        {
            ColCategory = ColliderCategory.EnemyBody,
            ActorName = "BossShip",
            Owner = this
        };
        ColliderManager.Instance.AddCollider(_rect);

        _timer = new();
        _timer.Initialize();

        MoveData = new EnemyDataStructs.TrackPlayer
        {
            MoveSpeed = 2.0f,
            Target = PlayerManager.Instance.Player,
            TurnRate = 80.0f,
        };

        _actions = new()
        {
            {
                ActionType.SpreadBarrage,
                new BossDataStructs.SpreadBarrage
                {
                    SummonEnemyVal = 5,
                    Transform = this.transform,
                    ActionInterval = 30.0f,
                    RemainInterval = 0.0f
                }
            },

            {
                ActionType.ShootMissile,
                new BossDataStructs.ShootMissiles
                {
                    ActionInterval = 0.2f,
                    Transform = this.transform,
                    ShotValue = 30
                }
            },

            {
                ActionType.DiscShot,
                new BossDataStructs.DiscShot
                {
                    ActionInterval = 0.75f,
                    Transform = this.transform,
                    ShotValue = 20,
                    LittleRing = new BulletStructs.RingShot
                    {
                        Acceleration = 0.1f,
                        DisAcceleration = 0.5f,
                        ColCategory = ColliderCategory.EnemyBullet,
                        Dir = this.transform.right,
                        MoveSpeed = 1.0f,
                        SecondMoveSpeed = 15.0f,
                        Scale = Vector3.one * 0.75f,
                        SpriteType = SpriteData.SpriteType.EnemyBullet
                    },
                    BigRing = new BulletStructs.RingShot
                    {
                        Acceleration = 0.0f,
                        DisAcceleration = 2.0f,
                        ColCategory = ColliderCategory.EnemyBullet,
                        Dir = this.transform.right,
                        MoveSpeed = 5.5f,
                        SecondMoveSpeed = 5.0f,
                        Scale = Vector3.one * 1.0f,
                        SpriteType = SpriteData.SpriteType.EnemyBullet
                    }
                }
            }
        };

        _actionPattern = new ActionType[]
        {
            ActionType.SpreadBarrage,
            ActionType.ShootMissile,
            ActionType.DiscShot,
        };

        foreach (var action in _actions.Values)
            action.Initialize();

        _currentPattern = 0;
        _currentAction = _actionPattern[_currentPattern];

        EnemyManager.Instance.CreateSpiralBarrierEnemy(
                    (int)EnemyEnums.EnemyID.渦巻ぐるぐる敵_自機狙い単発弾,
                    this.transform.position,
                    30,
                    20.0f,
                    UnityEngine.Random.Range(0, 100) % 2 == 0,
                    this.transform);

        EventDispatcher.Instance.Bind(this, _rect.ActorName);
    }

    private void FixedUpdate()
    {
        _timer.Update();

        _renderer.SetFlipX(MoveData.MoveDir.x < 0);

        var currentAction = _actions[_currentAction];

        if (currentAction.RemainInterval > 0.0f)
            currentAction.RemainInterval -= Time.fixedDeltaTime;
        else
            Action();

        if (currentAction.IsFinished())
            ChangeNextAction();

        if (_isInvincible)
        {
            if (_renderer.GetSpriteAlpha() < 0.1f)
                _renderer.SetSpriteAlpha(1.0f);
            else
                _renderer.SetSpriteAlpha(0.0f);
        }
        else if (_renderer.GetSpriteAlpha() < 0.1f)
            _renderer.SetSpriteAlpha(1.0f);
    }

    public void Action()
    {
        _actions[_currentAction].Act();
    }

    private void ChangeNextAction()
    {
        ++_currentPattern;
        if (_currentPattern >= _actionPattern.Length)
        {
            _currentPattern = 0;
        }
        _currentAction = _actionPattern[_currentPattern];
        _actions[_currentAction].Initialize();
    }

    [CallableEvent("OnTriggerEnter")]
    public void OnHit(object data)
    {
        if (_isInvincible)
            return;

        _isInvincible = true;

        --Life;

        if (Life < 1)
        {
            EventDispatcher.Instance.Unbind(this, _rect.ActorName);
            EventDispatcher.Instance.Dispatch("BossSmashed");
            _actions[_currentAction].OnDestroyed();
            return;
        }

        _timer.CreateTask(() => _isInvincible = false, _INVINCIBLETIME);
    }

    public void PlaySmashedEffect()
    {
        Vector2 rnd = Random.insideUnitCircle * 3.0f;
        var center = this.transform.position;
        Vector3 pos = new Vector3(center.x + rnd.x, center.y + rnd.y, center.z);
        var particle = Instantiate(_particle, pos, Quaternion.identity);
        CRISoundManager.Instance.PlaySE(SFX.BossExplosion2);

        --_effectCount;

        if (_effectCount > 0)
            _timer.CreateTask(PlaySmashedEffect, 0.1f);
        else
        {
            CRISoundManager.Instance.PlaySE(SFX.BossExplode);
            IsDestroyWaiting = true;
        }
    }

    public void DestroyByColliderManager()
    {
        ColliderManager.Instance.RemoveCollider(_rect);
        Destroy(this.gameObject);
    }
}