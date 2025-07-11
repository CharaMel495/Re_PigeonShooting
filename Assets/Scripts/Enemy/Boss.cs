using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ボスの本体クラス
/// </summary>
public class Boss : MonoBehaviour
{
    private enum ActionType
    {
        SpreadBarrage,
        ShootMissile,
        DiscShot
    }

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

    private int hoge = 0;

    public void Initialize()
    {
        _renderer.Initialize();
        _renderer.SetSprite(SpriteManager.GetSprite(SpriteData.SpriteType.BossShip));

        _rect = new(this.transform);
        _rect.ColCategory = ColliderCategory.EnemyBody;
        _rect.ActorName = "BossShip";
        ColliderManager.Instance.AddCollider(_rect);

        _timer = new();
        _timer.Initialize();

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
                    ActionInterval = 1.0f,
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
                        Scale = Vector3.one * 0.75f,
                        SpriteType = SpriteData.SpriteType.PlayerBullet
                    },
                    BigRing = new BulletStructs.RingShot
                    {
                        Acceleration = 0.0f,
                        DisAcceleration = 2.0f,
                        ColCategory = ColliderCategory.EnemyBullet,
                        Dir = this.transform.right,
                        MoveSpeed = 5.5f,
                        Scale = Vector3.one * 1.0f,
                        SpriteType = SpriteData.SpriteType.PlayerBullet
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

        EventDispatcher.Instance.Subscribe(EventNames.GetEventName(Events.OnHit, _rect.ActorName), OnHit);
    }

    private void FixedUpdate()
    {
        _timer.Update();

        var currentAction = _actions[_currentAction];

        if (currentAction.RemainInterval > 0.0f)
            currentAction.RemainInterval -= Time.fixedDeltaTime;
        else
            Action();

        if (currentAction.IsFinished())
            ChangeNextAction();

        this.transform.position = new Vector3
            (
                Mathf.Cos(Time.fixedTime) + 6,
                Mathf.Sin(Time.fixedTime)
            );

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
            Debug.Log(hoge);
        }
        _currentAction = _actionPattern[_currentPattern];
        _actions[_currentAction].Initialize();
    }

    public void OnHit(object data)
    {
        if (_isInvincible)
            return;

        _isInvincible = true;

        --Life;

        ++hoge;

        if (Life < 1)
        {
            //IsDestroyWaiting = true;
            return;
        }

        _timer.CreateTask(() => _isInvincible = false, _INVINCIBLETIME);
    }
}
