using UnityEngine;

/// <summary>
/// 方向指定用のenum
/// </summary>
public enum Direction
{
    None,
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft
}

/// <summary>
/// ゲーム全体の管理クラス
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    /// <summary>
    /// １フレームの秒数
    /// </summary>
    public static readonly float FRAME = 0.017f;

    /// <summary>
    /// 画面暗転フラグ
    /// </summary>
    public static bool IsFade
    { get; private set; } = false;

    public void Start()
    {
        Initialize();
        Application.targetFrameRate = 60;
    }

    public void Initialize()
    {
        MainSceneManager.Instance.Initialize();
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.Player))
            Time.timeScale = (Time.timeScale < 1.0 ? 1.0f : 0.0f);
    }

    /// <summary>
    /// ゲームを終了させる関数
    /// </summary>
    public static void EndGame()
    {
        //エディタから開いていたらエディタを終了
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //そうで無ければアプリを終了させる
        Application.Quit();
#endif
    }
}
