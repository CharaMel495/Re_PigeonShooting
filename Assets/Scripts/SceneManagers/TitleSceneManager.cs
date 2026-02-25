using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : SceneManagerBase<TitleSceneManager>
{
    private enum CurrentState
    {
        Top,
        TitleMenu,
        Option,
        Credit,
        Ranking,
        Exit
    }

    [SerializeField]
    private ButtonMenuController _titleMenu;

    [SerializeField]
    private ScoreBoardController _scoreBoard;

    [SerializeField]
    private LoadingCutIn _loadingCutin;

    [SerializeField]
    private GameObject _textObj;

    [SerializeField]
    private ScoreHolder _highScore;

    [SerializeField]
    private ScoreRanking _scoreData;

    [SerializeField]
    private ImageWrapper _creditImage;

    private CurrentState _state;

    private Dictionary<CurrentState, Action> _desideKeyPressed;
    private Dictionary<CurrentState, Action> _cancelKeyPressed;
    private Dictionary<CurrentState, Action<Direction>> _dirInputed;

    public override void Initialize()
    {
        _titleMenu.Initialize(CreateButtonFunc());

        _creditImage.Initialize();
        _creditImage.SetImageAlpha(0.0f);

        _scoreBoard.Initialize();

        _state = CurrentState.Top;

        _loadingCutin.ExitCutin();

        _textObj.SetActive(true);

        InputManager.Instance.ChangeInputHandler(InputHandler.UI);

        _desideKeyPressed = new Dictionary<CurrentState, Action>
        {
            { CurrentState.Top, OpenMenu },
            { CurrentState.TitleMenu, SelectMenu },
            { CurrentState.Option, null },
            { CurrentState.Credit, null },
            { CurrentState.Ranking, null },
            { CurrentState.Exit, null },
        };

        _cancelKeyPressed = new Dictionary<CurrentState, Action>
        {
            { CurrentState.Top, null },
            { CurrentState.TitleMenu, CloseMenu },
            { CurrentState.Option, null },
            { CurrentState.Credit, CloseCredit },
            { CurrentState.Ranking, CloseScoreBoard },
            { CurrentState.Exit, null },
        };

        _dirInputed = new Dictionary<CurrentState, Action<Direction>>
        {
            { CurrentState.Top, null },
            { CurrentState.TitleMenu, MoveMenu },
            { CurrentState.Option, null },
            { CurrentState.Credit, null },
            { CurrentState.Ranking, MoveScoreBoard },
            { CurrentState.Exit, null },
        };

        CRISoundManager.Instance.PlayBGM(BGM.Title);

        Action[] CreateButtonFunc()
        {
            return new Action[]
                {
                    // ゲーム開始
                    () => _loadingCutin.EnterCutin(() => SceneManager.LoadScene("MainGame")),
                    // 難易度変更
                    null,
                    // チュートリアル開始
                    () => _loadingCutin.EnterCutin(() => SceneManager.LoadScene("Tutorial")),
                    // オプション
                    null,
                    // クレジット
                    OpenCredit,
                    // スコアボード
                    OpenScoreBoard,
                    // ゲーム終了
                    () => _loadingCutin.EnterCutin(() => GameManager.EndGame()),
                };
        }
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.DesideKey, InputHandler.UI))
            _desideKeyPressed[_state]?.Invoke();

        if (InputManager.CheckKey(InputManager.CancelKey, InputHandler.UI))
            _cancelKeyPressed[_state]?.Invoke();

        // ポーズキー＋決定キー＋吸引キーでハイスコアリセット
        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.UI, true) && InputManager.CheckKey(InputManager.CancelKey, InputHandler.UI, true) && InputManager.CheckKey(InputManager.VacuumKey, InputHandler.UI))
        {
            _scoreData.ResetList();
            CRISoundManager.Instance.PlaySE(SFX.TutorialSuccess);
        }

        Direction inputDir = InputManager.CheckInputDirection(InputHandler.UI, true);

        if (inputDir == Direction.Down)
            _dirInputed[_state]?.Invoke(Direction.Down);

        if (inputDir == Direction.Up)
            _dirInputed[_state]?.Invoke(Direction.Up);

        if (inputDir == Direction.Right)
            _dirInputed[_state]?.Invoke(Direction.Right);

        if (inputDir == Direction.Left)
            _dirInputed[_state]?.Invoke(Direction.Left);
    }

    private void OpenMenu()
    {
        if (_titleMenu.EnActive())
            _state = CurrentState.TitleMenu;

        _textObj.SetActive(false);
    }

    private void CloseMenu()
    {
        if (_titleMenu.DisActive())
            _state = CurrentState.Top;

        _textObj.SetActive(true);
    }

    private void MoveMenu(Direction dir)
    {
        _titleMenu.MoveButton(dir);
    }

    private void SelectMenu()
    {
        _titleMenu.SelectButton();
    }

    private void OpenScoreBoard()
    {
        _scoreBoard.EnActive();
        _state = CurrentState.Ranking;
    }

    private void CloseScoreBoard()
    {
        _scoreBoard.DisActive();
        _state = CurrentState.TitleMenu;
    }

    private void OpenCredit()
    {
        _creditImage.SetImageAlpha(1.0f);
        _state = CurrentState.Credit;
    }

    private void CloseCredit()
    {
        _creditImage.SetImageAlpha(0.0f);
        _state = CurrentState.TitleMenu;
    }

    private void MoveScoreBoard(Direction dir)
    {
        if (dir != Direction.Up && dir != Direction.Down)
            return;

        bool isDown = dir == Direction.Down;

        _scoreBoard.Scroll(isDown);
    }
}
