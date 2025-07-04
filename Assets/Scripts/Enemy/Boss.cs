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

    private void Start()
        => Initialize();

    public void Initialize()
    {
        _renderer.Initialize();
        _renderer.SetSprite(SpriteManager.GetSprite(SpriteData.SpriteType.BossShip));

        _currentAction = new BossDataStructs.SpreadBarrage{
            SummonEnemyVal = 5,
            transform = this.transform,
            ActionInterval = 6.0f,
            RemainInterval = 0.0f
        };
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
    }

    public void Action()
    {
        _currentAction.Act();
    }
}
