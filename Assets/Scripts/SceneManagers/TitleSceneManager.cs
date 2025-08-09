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
    private LoadingCutIn _loadingCutin;

    [SerializeField]
    private GameObject _textObj;

    [SerializeField]
    private ScoreHolder _highScore;

    private CurrentState _state;

    private Dictionary<CurrentState, Action> _desideKeyPressed;
    private Dictionary<CurrentState, Action> _cancelKeyPressed;
    private Dictionary<CurrentState, Action<bool>> _dirInputed;

    public override void Initialize()
    {
        _titleMenu.Initialize(CreateButtonFunc());

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
            { CurrentState.Credit, null },
            { CurrentState.Ranking, null },
            { CurrentState.Exit, null },
        };

        _dirInputed = new Dictionary<CurrentState, Action<bool>>
        {
            { CurrentState.Top, null },
            { CurrentState.TitleMenu, MoveMenu },
            { CurrentState.Option, null },
            { CurrentState.Credit, null },
            { CurrentState.Ranking, null },
            { CurrentState.Exit, null },
        };

        CRISoundManager.Instance.PlayBGM(BGM.Title);

        Action[] CreateButtonFunc()
        {
            return new Action[]
                {
                    () => _loadingCutin.EnterCutin(() => SceneManager.LoadScene("Tutorial")),
                    null,
                    null,
                    null,
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
            _highScore.Initialize();
            CRISoundManager.Instance.PlaySE(SFX.TutorialSuccess);
        }

        Direction inputDir = InputManager.CheckInputDirection(InputHandler.UI);

        if (inputDir == Direction.Down)
            _dirInputed[_state]?.Invoke(true);

        if (inputDir == Direction.Up)
            _dirInputed[_state]?.Invoke(false);
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

    private void MoveMenu(bool isDown)
    {
        _titleMenu.MoveButton(isDown);
    }

    private void SelectMenu()
    {
        _titleMenu.SelectButton();
    }
}
