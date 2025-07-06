using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボスの本体クラス
/// </summary>
public class Boss : MonoBehaviour
{
    /// <summary>
    /// 現在のアクション
    /// </summary>
    public BossDataStructs.IBossAction _currentAction;

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

        _currentAction = new BossDataStructs.SpreadBarrage{
            SummonEnemyVal = 5,
            transform = this.transform,
            ActionInterval = 10.0f,
            RemainInterval = 0.0f
        };

        _currentAction.Initialize();

        EventDispatcher.Instance.Subscribe(EventNames.GetEventName(Events.OnHit, _rect.ActorName), OnHit);
    }

    private void FixedUpdate()
    {
        if (_currentAction.RemainInterval > 0.0f)
            _currentAction.RemainInterval -= Time.fixedDeltaTime;
        else
            Action();

        this.transform.position = new Vector3
            (
                Mathf.Cos(Time.fixedTime) + 3,
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
        _currentAction.Act();
    }

    public void OnHit(object data)
    {
        if (_isInvincible)
            return;

        _isInvincible = true;

        --Life;

        if (Life < 1)
        {
            //IsDestroyWaiting = true;
            return;
        }

        _timer.CreateTask(() => _isInvincible = false, _INVINCIBLETIME);
    }
}
