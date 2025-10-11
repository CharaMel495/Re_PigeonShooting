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

    [SerializeField]
    private ScoreHolder _scoreHolder;

    [SerializeField]
    private TextWrapper _scoreText;

    private bool _isPausing = false;

    public override void Initialize()
    {
        ColliderManager.Instance.Initialize();
        BulletManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        EnemyManager.Instance.Initialize();
        StageManager.Instance.Initialize();

        _pauseMenu.Initialize(CreateButtonFunc());

        _scoreHolder.Initialize();

        _loadingCutin.ExitCutin();

        CRISoundManager.Instance.PlayBGM(BGM.MainStage);

        EventDispatcher.Instance.Bind(this);

        InputManager.Instance.ChangeInputHandler(InputHandler.Player);

        _scoreText.Initialize();
        _scoreText.SetText($"{_scoreHolder.Score:D8}pts");

        //CRISoundManager.Instance.PlayVoice(Voice.Start_Voice);

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
        _scoreHolder.Score += (int)data;
        _scoreText.SetText($"{_scoreHolder.Score:D8}pts");
    }

    [CallableEvent("GameOver")]
    public void GameOver(object data)
    {
        _loadingCutin.EnterCutin(() => SceneManager.LoadScene("GameOver"));
    }
}
