using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// メインシーンについて管理するクラス
/// </summary>
public class MainSceneManager : SceneManagerBase<MainSceneManager>
{
    [SerializeField]
    private LoadingCutIn _cutinUI;
    public LoadingCutIn CutInUI
        => _cutinUI;

    [SerializeField]
    private LoadingCutIn _loadingCutin;

    private static int _score;

    public override void Initialize()
    {
        ColliderManager.Instance.Initialize();
        BulletManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        EnemyManager.Instance.Initialize();
        StageManager.Instance.Initialize();

        _score = 0;

        _loadingCutin.ExitCutin();

        CRISoundManager.Instance.PlayBGM(BGM.MainStage);

        EventDispatcher.Instance.Bind(this);
    }

    private void Update()
    {
        if (InputManager.CheckKey(KeyCode.E, InputHandler.Player))
        {
            EnemyManager.Instance.CreateSpiralEnemy(new Vector3(3.0f, 0.0f, 0.0f), 2, false);
            EnemyManager.Instance.CreateSpiralEnemy(new Vector3(3.0f, 0.0f, 0.0f), 2, true);

        }
    }

    [CallableEvent("AddScore")]
    public void AddScore(object data)
    {
        _score += (int)data;
    }

    [CallableEvent("GameOver")]
    public void GameOver(object data)
    {
        _loadingCutin.EnterCutin(() => SceneManager.LoadScene("GameOver"));
    }
}
