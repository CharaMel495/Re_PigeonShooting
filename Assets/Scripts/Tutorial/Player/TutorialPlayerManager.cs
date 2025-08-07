using System.Collections.Generic;
using UnityEngine;
using Tutorial;

public class TutorialPlayerManager : SingletonMonoBehaviour<TutorialPlayerManager>
{
    [SerializeField]
    [Header("プレイヤー")]
    private TutorialPlayer _player;
    /// <summary>
    /// プレイヤー
    /// </summary>
    public TutorialPlayer Player => _player;

    /// <summary>
    /// プレイヤーを移動させるクラス
    /// </summary>
    private TutorialPlayerMover _mover;

    [SerializeField]
    private float _moveSpeed;

    private bool _isMovable = true;

    /// <summary>
    /// 初期化を行う関数
    /// </summary>
    public void Initialize()
    {
        _player.Initialize();
        _player.Shooter = BulletManager.Instance.Shooter;
        _mover = new(_moveSpeed, TutorialStageManager.Instance.PlayArea);
        _mover.Initialize(_moveSpeed, TutorialStageManager.Instance.PlayArea);

        _player.DisActive();
    }

    private void Update()
    {
        if (!_player.IsActive)
            return;

        //プレイヤーに関係するイベントの発火を見張る
        CheckPlayerEvent();
    }

    private void FixedUpdate()
    {
        if (!_player.IsActive || !_isMovable)
            return;

        _mover.MovePlayer(_player);
    }

    /// <summary>
    /// プレイヤーに関係するイベントを見張るメソッド
    /// </summary>
    private void CheckPlayerEvent()
    {
        if (InputManager.IsShotKeyDowning(out Vector2 joyStickMap, out float slopeCondition, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnShotKeyPressed, "Player"), _player.GetBulletParameter(joyStickMap, slopeCondition));

        if (InputManager.CheckKey(InputManager.VacuumKey, InputHandler.Player, isPrevious: true))
            if (InputManager.CheckKey(InputManager.DashKey, InputHandler.Player))
                EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnAirBasterKeyPressed, "Player"));
            else
                EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnVacuumKeyPressed, "Player"));
        else
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnVacuumKeyReleased, "Player"));


        if (InputManager.CheckKey(InputManager.DashKey, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnDashKeyPressed, "Player"));

        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnMenuKeyPressed, "Player"));
    }

    public void EnActivePlayer(Dictionary<Tutorials, bool> arrowedActions)
    {
        _isMovable = arrowedActions[Tutorials.Move];
        _player.EnActive(arrowedActions);
    }
}
