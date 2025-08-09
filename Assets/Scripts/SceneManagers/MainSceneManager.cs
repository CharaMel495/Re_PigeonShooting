using System;
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

    [SerializeField]
    private ButtonMenuController _pauseMenu;

    private bool _isPausing = false;

    private static int _score;

    public override void Initialize()
    {
        ColliderManager.Instance.Initialize();
        BulletManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        EnemyManager.Instance.Initialize();
        StageManager.Instance.Initialize();

        _pauseMenu.Initialize(CreateButtonFunc());

        _score = 0;

        _loadingCutin.ExitCutin();

        CRISoundManager.Instance.PlayBGM(BGM.MainStage);

        EventDispatcher.Instance.Bind(this);

        Action[] CreateButtonFunc()
        {
            return new Action[]
                {
                    ExitPauseMode,
                    null,
                    BackToTitile
                };
        }
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.Player))
        {
            EnterPauseMode();
        }

        if (InputManager.CheckKey(InputManager.DesideKey, InputHandler.UI))
            _pauseMenu.SelectButton();

        if (InputManager.CheckKey(InputManager.CancelKey, InputHandler.UI))
            ExitPauseMode();

        Direction inputDir = InputManager.CheckInputDirection(InputHandler.UI);

        if (inputDir == Direction.Down)
            _pauseMenu.MoveButton(isDown: true);

        if (inputDir == Direction.Up)
            _pauseMenu.MoveButton(isDown: false);
    }

    private void EnterPauseMode()
    {
        _isPausing = true;
        InputManager.Instance.ChangeInputHandler(InputHandler.UI);
        _pauseMenu.EnActive();
        Time.timeScale = 0.0f;
    }

    private void ExitPauseMode()
    {
        _isPausing = false;
        InputManager.Instance.ChangeInputHandler(InputHandler.Player);
        _pauseMenu.DisActive();
        Time.timeScale = 1.0f;
    }

    private void BackToTitile()
    {
        _loadingCutin.EnterCutin(() => { ExitPauseMode(); SceneManager.LoadScene("Title"); });
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
