using System;
using System.Collections.Generic;
using UnityEngine;
using Tutorial;

namespace Tutorial
{
    public enum Tutorials
    {
        Move,
        Shot,
        Vacuum,
        Dash,
        AirBaster
    }
}

public class TutorialSceneManager : SceneManagerBase<TutorialSceneManager>
{
    private enum CurrentState
    {
        Top,
        ExamTutorial,
        PlayTutorial,
        EndTutorial,
        Loading,
    }

    [SerializeField]
    private ButtonMenuController _tutorialMenu;

    [SerializeField]
    private LoadingCutIn _loadingCutin;

    private CurrentState _state;

    private Dictionary<CurrentState, Action> _desideKeyPressed;
    private Dictionary<CurrentState, Action> _cancelKeyPressed;
    private Dictionary<CurrentState, Action<Direction>> _dirInputed;

    private int _lastSelectedTutrial = 0;

    private TutorialSupporter _supporter;

    public override void Initialize()
    {
        _state = CurrentState.Loading;

        _tutorialMenu.Initialize(CreateButtonFunc());

        _tutorialMenu.EnActive();

        _desideKeyPressed = new Dictionary<CurrentState, Action>
        {
            { CurrentState.Top, _tutorialMenu.SelectButton },
            { CurrentState.ExamTutorial, null },
            { CurrentState.PlayTutorial, null },
            { CurrentState.EndTutorial, null },
            { CurrentState.Loading, null },
        };

        _cancelKeyPressed = new Dictionary<CurrentState, Action>
        {
            { CurrentState.Top, null },
            { CurrentState.ExamTutorial, null },
            { CurrentState.PlayTutorial, null },
            { CurrentState.EndTutorial, null },
            { CurrentState.Loading, null },
        };

        _dirInputed = new Dictionary<CurrentState, Action<Direction>>
        {
            { CurrentState.Top, MoveMenu },
            { CurrentState.ExamTutorial, null },
            { CurrentState.PlayTutorial, null },
            { CurrentState.EndTutorial, null },
            { CurrentState.Loading, null },
        };

        _supporter = new();

        TutorialStageManager.Instance.Initialize();
        ColliderManager.Instance.Initialize();
        BulletManager.Instance.Initialize();
        TutorialPlayerManager.Instance.Initialize();

        _loadingCutin.ExitCutin(() => _state = CurrentState.Top);

        Action[] CreateButtonFunc()
        {
            return new Action[]
                {
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Move)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Shot)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Vacuum)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Dash)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.AirBaster)),
                    null,
                };
        }
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.DesideKey, InputHandler.Player))
            _desideKeyPressed[_state]?.Invoke();

        if (InputManager.CheckKey(InputManager.CancelKey, InputHandler.Player))
            _cancelKeyPressed[_state]?.Invoke();

        Direction inputDir = InputManager.CheckInputDirection(InputHandler.Player, isPrevious: true);

        _dirInputed[_state]?.Invoke(inputDir);
    }

    private void MoveMenu(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                _tutorialMenu.MoveButton(_lastSelectedTutrial);
                break;

            case Direction.Down:
                _lastSelectedTutrial = _tutorialMenu.CurrentButton;
                _tutorialMenu.MoveButton(5);
                break;

            case Direction.Right:

                if (_tutorialMenu.CurrentButton >= 4)
                    break;

                _tutorialMenu.MoveButton(true);
                _lastSelectedTutrial = _tutorialMenu.CurrentButton;
                break;

            case Direction.Left:
                _tutorialMenu.MoveButton(false);
                _lastSelectedTutrial = _tutorialMenu.CurrentButton;
                break;
        }
    }

    private void SetUpTutorial(Tutorials tutorial)
    {

    }
}
