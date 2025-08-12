using System;
using System.Collections.Generic;
using Tutorial;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private TutorialUI _tutorialUI;

    [SerializeField]
    private TutorialChecker _tutorialChecker;

    [SerializeField]
    private Item _itemPrefab;

    [SerializeField]
    private TextWrapper _topText;

    private CurrentState _state;

    private Dictionary<CurrentState, Action> _desideKeyPressed;
    private Dictionary<CurrentState, Action> _cancelKeyPressed;
    private Dictionary<CurrentState, Action<Direction>> _dirInputed;

    private int _lastSelectedTutrial = 0;

    private TutorialSupporter _supporter;

    public bool IsAirBasterOK = false;

    public override void Initialize()
    {
        InputManager.Instance.ChangeInputHandler(InputHandler.UI);

        _state = CurrentState.Loading;

        _tutorialMenu.Initialize(CreateButtonFunc());

        _tutorialMenu.EnActive();

        _tutorialChecker.Initialize();

        _supporter = new(_tutorialUI);

        _topText.Initialize();

        _desideKeyPressed = new Dictionary<CurrentState, Action>
        {
            { CurrentState.Top, _tutorialMenu.SelectButton },
            { CurrentState.ExamTutorial, () => _supporter.PlayNext() },
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

        TutorialStageManager.Instance.Initialize();
        ColliderManager.Instance.Initialize();
        BulletManager.Instance.Initialize();
        TutorialPlayerManager.Instance.Initialize();
        EnemyManager.Instance.Initialize();

        _loadingCutin.ExitCutin(() => _state = CurrentState.Top);

        EventDispatcher.Instance.Bind(this);

        CRISoundManager.Instance.PlayBGM(BGM.Tutorial);

        Action[] CreateButtonFunc()
        {
            return new Action[]
                {
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Move)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Shot)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Vacuum)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.Dash)),
                    () => _loadingCutin.EnterCutin(() => SetUpTutorial(Tutorials.AirBaster)),
                    () => _loadingCutin.EnterCutin(() => SceneManager.LoadScene("MainGame")),
                };
        }
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.DesideKey, InputHandler.UI))
            _desideKeyPressed[_state]?.Invoke();

        if (InputManager.CheckKey(InputManager.CancelKey, InputHandler.UI))
            _cancelKeyPressed[_state]?.Invoke();

        Direction inputDir = InputManager.CheckInputDirection(InputHandler.UI, isPrevious: true);

        _dirInputed[_state]?.Invoke(inputDir);

        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.UI))
            _loadingCutin.EnterCutin(BackToTop);
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
        _tutorialMenu.DisActive();
        _state = CurrentState.Loading;
        _supporter.SetUp(tutorial);
        _loadingCutin.ExitCutin(StartTutorial);
        _tutorialChecker.SetUp(tutorial);
        _topText.SetTextAlpha(0.0f);
    }

    public void StartTutorial()
    {
        _state = CurrentState.ExamTutorial;
        _supporter.PlayNext();
    }

    [CallableEvent("EndTutorial")]
    public void EndTutorial(object data)
    {
        _state = CurrentState.Loading;
        _loadingCutin.EnterCutin(() => BackToTop());
    }

    private void BackToTop()
    {
        if (_state == CurrentState.Top)
            return;

        _tutorialMenu.EnActive(_tutorialMenu.CurrentButton);

        EventDispatcher.Instance.Dispatch("PlayerEndTutorial");

        InputManager.Instance.ChangeInputHandler(InputHandler.UI);

        _state = CurrentState.Loading;

        _supporter.ClearnUp();

        _loadingCutin.ExitCutin(() => _state = CurrentState.Top);

        _topText.SetTextAlpha(1.0f);
    }

    [CallableEvent("EnterPlayingMode")]
    public void EnterPlayingMode(object data)
    {
        _state = CurrentState.PlayTutorial;
        TutorialSpawn(_tutorialChecker.EnterCheckingMode());
    }

    [CallableEvent("CorrectTutorial")]
    public void CorrectTutorial(object data)
    {
        _state = CurrentState.ExamTutorial;
        _supporter.PlayNext();
    }

    private void TutorialSpawn(CheckLists checkTarget)
    {
        switch (checkTarget)
        {
            case CheckLists.Move_ItemGet:
                {
                    var playerPos = TutorialPlayerManager.Instance.Player.GetPosition();
                    var spawnPos = playerPos + (Vector3.left * 3);
                    var item = Instantiate(_itemPrefab, spawnPos, Quaternion.identity);
                    item.Initialize(ItemType.Battery_Green);
                }
                break;

            case CheckLists.Shot_SmashEnemy:
                {
                    var playerPos = TutorialPlayerManager.Instance.Player.GetPosition();
                    var spawnPos = playerPos + (Vector3.right * 3);
                    EnemyManager.Instance.CreateEnemy(12, spawnPos);
                }
                break;

            case CheckLists.Vacuum_Item:
                {
                    var playerPos = TutorialPlayerManager.Instance.Player.GetPosition();
                    var spawnPos = playerPos + (Vector3.right * 3);
                    var item = Instantiate(_itemPrefab, spawnPos, Quaternion.identity);
                    item.Initialize(ItemType.Battery_Red);
                }
                break;

            case CheckLists.AirBaster_GetDust:
                {
                    int count = 10;
                    float radius = 3f;

                    var playerPos = TutorialPlayerManager.Instance.Player.GetPosition();

                    for (int i = 0; i < count; ++i)
                    {
                        float angleDeg = 360f * i / count;
                        // ① 右方向の単位ベクトルを角度だけ回す
                        Vector3 offset = Quaternion.AngleAxis(angleDeg, Vector3.forward) * Vector3.right * radius;
                        // ② プレイヤー位置に足し戻してスポーン
                        Vector3 spawnPos = playerPos + offset;

                        var item = Instantiate(_itemPrefab, spawnPos, Quaternion.identity);
                        item.Initialize(ItemType.Garbage);
                    }
                }
                break;

            case CheckLists.AirBaster:
                {
                    var playerPos = TutorialPlayerManager.Instance.Player.GetPosition();
                    int count = 8;
                    float radius = 3f;

                    for (int i = 0; i < count; ++i)
                    {
                        float angleDeg = 360f * i / count;
                        // ① 右方向の単位ベクトルを角度だけ回す
                        Vector3 offset = Quaternion.AngleAxis(angleDeg, Vector3.forward) * Vector3.right * radius;
                        // ② プレイヤー位置に足し戻してスポーン
                        Vector3 spawnPos = playerPos + offset;

                        EnemyManager.Instance.CreateEnemy(12, spawnPos);
                    }


                    EventDispatcher.Instance.Dispatch("PlayerIsAirBasterUnLock");
                }
                break;
        }
    }
}
