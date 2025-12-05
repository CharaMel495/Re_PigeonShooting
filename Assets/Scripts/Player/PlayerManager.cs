using UnityEngine;

/// <summary>
/// プレイヤーを管理するクラス
/// </summary>
public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    [SerializeField]
    [Header("プレイヤー")]
    private Player _player;
    /// <summary>
    /// プレイヤー
    /// </summary>
    public Player Player => _player;

    /// <summary>
    /// プレイヤーを移動させるクラス
    /// </summary>
    private PlayerMover _mover;

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private BackGroundScroller _bgScroller;

    [SerializeField]
    private PlayerUI _playerUI;

    /// <summary>
    /// 初期化を行う関数
    /// </summary>
    public void Initialize()
    {
        _player.Initialize();
        _player.Shooter = BulletManager.Instance.Shooter;
        _mover = new(_moveSpeed);
        _mover.Initialize(_moveSpeed, StageManager.Instance.PlayArea);
        _playerUI.Initialize();

        EventDispatcher.Instance.Bind(this);
    }

    private void Update()
    {
        //プレイヤーに関係するイベントの発火を見張る
        CheckPlayerEvent();   
    }

    private void FixedUpdate()
    {
        _mover.MovePlayer(_player, out Vector3 moveValue);
        if (!EnemyManager.Instance.IsBossMode)
            _bgScroller.UpdateOffset(moveValue);
    }

    [CallableEvent("BossSmashed")]
    public void BossSmashed(object data)
    {
        _player.EnterInvincible();
    }

    /// <summary>
    /// プレイヤーに関係するイベントを見張るメソッド
    /// </summary>
    private void CheckPlayerEvent()
    {
        if (_player.IsDestroyWaiting)
            return;

        if (InputManager.IsShotKeyDowning(out Vector2 joyStickMap, out float slopeCondition, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnShotKeyPressed, "Player"), _player.GetBulletParameter(joyStickMap, slopeCondition));

        if (InputManager.CheckKey(InputManager.VacuumKey, InputHandler.Player, isPrevious: true) ||
            InputManager.IsRTriggerDown(InputHandler.Player))
             EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnVacuumKeyPressed, "Player"));
        else
             EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnVacuumKeyReleased, "Player"));

        if (InputManager.CheckKey(InputManager.BombKey, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnAirBasterKeyPressed, "Player"));            

        if (InputManager.CheckKey(InputManager.DashKey, InputHandler.Player) ||
            InputManager.IsLTriggerPressed(InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnDashKeyPressed, "Player"));

        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnMenuKeyPressed, "Player"));
    }
}
