using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

/// <summary>
/// 現在誰に入力情報を渡すか
/// </summary>
public enum InputHandler
{
    Player,
    UI
}

/// <summary>
/// 入力に関係する記述はすべてここに
/// </summary>
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    /// <summary>
    /// ゲームパッド操作か？
    /// </summary>
    public static bool IsGamePadMode
    { get; private set; }

    /// <summary>
    /// 現在入力情報を受け取る権限を持っているクラス
    /// </summary>
    public static InputHandler CurrentHandler
    { get; private set; } = InputHandler.Player;

    /// <summary>
    /// 一つ前のハンドラーの情報
    /// </summary>
    private static InputHandler _remainingHandler;

    /// <summary>
    /// ボムキー
    /// </summary>
    public static KeyCode BombKey
    { get => IsGamePadMode ? KeyCode.JoystickButton9 : KeyCode.C; }

    /// <summary>
    /// 決定
    /// </summary>
    public static KeyCode DesideKey
    { get => IsGamePadMode ? KeyCode.JoystickButton0 : KeyCode.Z; }

    /// <summary>
    /// キャンセル
    /// </summary>
    public static KeyCode CancelKey
    { get => IsGamePadMode ? KeyCode.JoystickButton1 : KeyCode.X; }

    /// <summary>
    /// 吸引キー
    /// </summary>
    public static KeyCode VacuumKey
    { get => IsGamePadMode ? KeyCode.JoystickButton4 : KeyCode.Space; }

    /// <summary>
    /// 回避ダッシュキー
    /// </summary>
    public static KeyCode DashKey
    { get => IsGamePadMode ? KeyCode.JoystickButton5 : KeyCode.LeftShift; }

    /// <summary>
    /// キャンセルキー
    /// </summary>
    public static KeyCode SlowKey
    { get => IsGamePadMode ? KeyCode.JoystickButton2 : KeyCode.LeftControl; }


    /// <summary>
    /// ポーズキー
    /// </summary>
    public static KeyCode PauseKey
    { get => IsGamePadMode ? KeyCode.JoystickButton7 : KeyCode.Escape; }

    // --- トリガーの状態保持用 ---
    private static float _prevRTriggerValue = 0f;
    private static float _prevLTriggerValue = 0f;

    // --- トリガーを押した・離した閾値 ---
    private const float _TRIGGER_THRESHOLD = 0.1f;

    /// <summary>
    /// Rトリガーが押された瞬間か？
    /// </summary>
    public static bool IsRTriggerPressed(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return false;

        float now = Input.GetAxisRaw("RTrigger");
        bool isPressed = now > _TRIGGER_THRESHOLD && _prevRTriggerValue <= _TRIGGER_THRESHOLD;
        _prevRTriggerValue = now;
        return isPressed;
    }

    /// <summary>
    /// Rトリガーが離された瞬間か？
    /// </summary>
    public static bool IsRTriggerReleased(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return false;

        float now = Input.GetAxisRaw("RTrigger");
        bool isReleased = now <= _TRIGGER_THRESHOLD && _prevRTriggerValue > _TRIGGER_THRESHOLD;
        _prevRTriggerValue = now;
        return isReleased;
    }

    /// <summary>
    /// Lトリガーが押された瞬間か？
    /// </summary>
    public static bool IsLTriggerPressed(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return false;

        float now = Input.GetAxisRaw("LTrigger");
        bool isPressed = now > _TRIGGER_THRESHOLD && _prevLTriggerValue <= _TRIGGER_THRESHOLD;
        _prevLTriggerValue = now;
        return isPressed;
    }

    /// <summary>
    /// Lトリガーが離された瞬間か？
    /// </summary>
    public static bool IsLTriggerReleased(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return false;

        float now = Input.GetAxisRaw("LTrigger");
        bool isReleased = now <= _TRIGGER_THRESHOLD && _prevLTriggerValue > _TRIGGER_THRESHOLD;
        _prevLTriggerValue = now;
        return isReleased;
    }


    public static bool IsRTriggerDown(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return false;

        return Input.GetAxisRaw("RTrigger") > Mathf.Epsilon; 
    }

    public static float GetRTriggerCondition(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return 0.0f;

        return Input.GetAxis("RTrigger");
    }

    public static bool IsLTriggerDown(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return false;

        return Input.GetAxisRaw("LTrigger") > Mathf.Epsilon;
    }

    public static float GetLTriggerCondition(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return 0.0f;

        return Input.GetAxis("LTrigger");
    }

    private Timer _timer;

    private static Direction _dirPreview = Direction.None;

    private static Vector2 _inputDir;

    private static float _STICKDEADZONE_SQRT = 0.1f;

    /// <summary>
    /// 初期化
    /// </summary>
    public async Task Initialize()
    {
        CurrentHandler = InputHandler.Player;
        _remainingHandler = CurrentHandler;
        _timer = new();
    }

    private void Update()
    {
        _timer?.Update();

        IsGamePadMode = CheckConnectController();
    }

    public static bool IsShotKeyDowning(out Vector2 joyStickMap, out float slopeCondition, InputHandler handle)
    {
        if (CurrentHandler != handle)
        {
            joyStickMap = Vector2.zero;
            slopeCondition = 0.0f;
            return false;
        }

        float inputValX = Input.GetAxis("RightStickHorizontal");
        float inputValY = Input.GetAxis("RightStickVertical");

        joyStickMap.x = inputValX;
        joyStickMap.y = -inputValY;

        slopeCondition = Mathf.Min(joyStickMap.magnitude, 1);
        joyStickMap.Normalize();

        return slopeCondition > _STICKDEADZONE_SQRT;
    }

    private bool CheckConnectController()
    {
        // 接続されているコントローラの名前を調べる
        var controllerNames = Input.GetJoystickNames();

        if (controllerNames.Length < 1)
            return false;

        // 一台もコントローラが接続されていなければfalse
        return controllerNames[0] != "";
    }

    /// <summary>
    /// 押されたキーを取得する関数
    /// </summary>
    /// <returns>入力されたキー</returns>
    public static KeyCode CheckPressedButton(InputHandler handle, bool isPrevious = true)
    {
        //現在のハンドラー以外には入力通知を渡さない
        //※入力無しと通知する
        if (CurrentHandler != handle)
            return KeyCode.None;

        //プレイヤーがハンドラーの場合でも、暗転中は入力を通知しない
        if (handle == InputHandler.Player && GameManager.IsFade)
            return KeyCode.None;

        //押されたキーの格納先変数
        KeyCode pressedCode = KeyCode.None;

        //押されたキーが何なのかを調べる
        foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
        {
            //もし押されたキーが分かったら
            if (isPrevious && Input.GetKeyDown(code) || !isPrevious && Input.GetKey(code))
            {
                //キーの情報を格納
                pressedCode = code;

                //見つかったのでここでループを打ち切る
                break;
            }
        }

        //入力されたキーを確認出来たので
        //それを返却する
        return pressedCode;
    }

    /// <summary>
    /// 入力方向を取得する関数
    /// </summary>
    /// <returns>入力方向</returns>
    public static Direction CheckInputDirection(InputHandler handle, bool isPrevious = false)
    {
        //現在のハンドラー以外には入力通知を渡さない
        //※入力無しと通知する
        if (CurrentHandler != handle)
            return Direction.None;

        //プレイヤーがハンドラーの場合でも、暗転中は入力を通知しない
        if (handle == InputHandler.Player && GameManager.IsFade)
            return Direction.None;

        float inputValX = Input.GetAxisRaw("Horizontal");
        float inputValY = Input.GetAxisRaw("Vertical");

        var dir = GetDirection();

        if (isPrevious && _dirPreview == dir)
            return Direction.None;

        _dirPreview = dir;

        return dir;

        Direction GetDirection()
        {
            return (Mathf.Abs(inputValX), Mathf.Abs(inputValY)) switch
            {
                ( < 0.5f, < 0.5f) => Direction.None,
                ( > 0, < 0.5f) => inputValX > 0 ? Direction.Right : Direction.Left,
                ( < 0.5f, > 0) => inputValY > 0 ? Direction.Up : Direction.Down,
                _ => inputValX > 0 ? inputValY > 0 ? Direction.UpRight : Direction.DownRight : inputValY > 0 ? Direction.UpLeft : Direction.DownLeft,
            };
        }
    }

    /// <summary>
    /// 入力を二軸で受け取るメソッド
    /// </summary>
    /// <param name="handle">呼び出し元ハンドル</param>
    /// <returns>二軸の入力値</returns>
    public static Vector2 GetInputDirection(InputHandler handle)
    {
        if (CurrentHandler != handle)
            return Vector2.zero;

        _inputDir.x = Input.GetAxisRaw("Horizontal");
        _inputDir.y = Input.GetAxisRaw("Vertical");

        return _inputDir;
    }

    /// <summary>
    /// 指定キーが入力されているかを調べる関数
    /// </summary>
    /// <returns>入力されたキー</returns>
    public static bool CheckKey(KeyCode checkKey, InputHandler handle, bool isPrevious = false)
    {
        //現在のハンドラー以外には入力通知を渡さない
        //※入力無しと通知する
        if (CurrentHandler != handle)
            return false;

        //プレイヤーがハンドラーの場合でも、暗転中は入力を通知しない
        if (handle == InputHandler.Player && GameManager.IsFade)
            return false;

        //入力されたキーを確認出来たので
        //それを返却する
        return isPrevious ? Input.GetKey(checkKey) : Input.GetKeyDown(checkKey);
    }

    /// <summary>
    /// 権限を移動させる関数
    /// </summary>
    /// <param name="nextHandler">次のハンドラー</param>
    public void ChangeInputHandler(InputHandler nextHandler)
    {
        _remainingHandler = CurrentHandler;
        _timer.CreateTask(() => SwapNextHandle(nextHandler), GameManager.FRAME);
    }

    public void SwapNextHandle(InputHandler nextHandler)
        => CurrentHandler = nextHandler;

    /// <summary>
    /// 一つ前のハンドラーにハンドルを返す関数
    /// </summary>
    public void ReturnHandle()
        => CurrentHandler = _remainingHandler;
}
