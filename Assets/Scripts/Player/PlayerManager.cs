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

    /// <summary>
    /// 初期化を行う関数
    /// </summary>
    public void Initialize()
    {
        _player.Initialize();
        _player.Shooter = BulletManager.Instance.Shooter;
        _mover = new(_moveSpeed, StageManager.Instance.PlayArea);
        _mover.Initialize(_moveSpeed, StageManager.Instance.PlayArea);
    }

    private void Update()
    {
        //プレイヤーに関係するイベントの発火を見張る
        CheckPlayerEvent();   
    }

    private void FixedUpdate()
    {
        _mover.MovePlayer(_player);
    }

    /// <summary>
    /// プレイヤーに関係するイベントを見張るメソッド
    /// </summary>
    private void CheckPlayerEvent()
    {
        if (InputManager.IsShotKeyDowning(out Vector2 joyStickMap, out float slopeCondition))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnShotKeyPressed, "Player"), _player.GetBulletParameter(joyStickMap, slopeCondition));

        if (InputManager.CheckKey(InputManager.SubShotKey, InputHandler.Player, isPrevious: true))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnSubShotKeyPressed, "Player"), _player.GetSubBulletParameter());

        if (InputManager.CheckKey(InputManager.BombKey, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnBombKeyPressed, "Player"));

        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.Player))
            EventDispatcher.Instance.Dispatch(EventNames.GetEventName(Events.OnMenuKeyPressed, "Player"));
    }
}
