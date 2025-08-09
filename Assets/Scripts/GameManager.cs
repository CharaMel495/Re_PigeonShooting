using UnityEngine;
using UnityEngine.SceneManagement;

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

public enum Scenes
{
    Title,
    MainScene,
    Tutorial,
    GameOver
}

public class UsableMethods
{
    /// <summary>
    /// ランダムな単位ベクトル（方向ベクトル）を返す（2D）
    /// </summary>
    public static Vector2 GetRandomDirection2D()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f); // 0〜360度のランダム角度（ラジアン）
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}

/// <summary>
/// ゲーム全体の管理クラス
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField]
    private Scenes _currentScene;

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
        EventDispatcher.Instance.Initialize();
        _ = InputManager.Instance.Initialize();
        _ = CRISoundManager.Instance.Initialize();
        InitializeScene();
    }

    private void InitializeScene()
    {
        switch (_currentScene)
        {
            case Scenes.Title:
                TitleSceneManager.Instance.Initialize();
                break;
            case Scenes.MainScene:
                MainSceneManager.Instance.Initialize();
                break;
            case Scenes.Tutorial:
                TutorialSceneManager.Instance.Initialize();
                break;
            case Scenes.GameOver:
                GameOverScene.Instance.Initialize();
                break;
        }
    }

    private void Update()
    {

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
