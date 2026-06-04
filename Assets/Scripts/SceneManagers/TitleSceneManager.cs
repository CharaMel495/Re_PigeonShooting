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

    [Header("タイトル画面")]
    [SerializeField]
    private GameObject _textObj;
    
    [Header("メインメニュー")]
    [SerializeField]
    private ButtonMenuController _titleMenu;

    [Header("立札")]
    [SerializeField]
    private ImageWrapper _boardBG;
    [SerializeField]
    private ImageWrapper _boardFR;
    [SerializeField]
    private Sprite[] _inBoardSprites;

    [Header("メッセージボックス")]
    [SerializeField]
    private ImageWrapper _talkBG;
    [SerializeField]
    private TalkData _talkData;
    [SerializeField]
    private TextWrapper _talkTextUI;
    [SerializeField]
    private ImageWrapper _talkFace;

    [Header("難易度周りの追加UI")]
    [SerializeField]
    private TextWrapper _difficultyTextUI;
    [SerializeField]
    [TextArea]
    private string[] _difficultyTexts;
    [SerializeField]
    private ChallengeData _challengeData;

    [Header("スコアボード")]
    [SerializeField]
    private ScoreBoardController _scoreBoard;

    [Header("クレジット画面")]
    [SerializeField]
    private ImageWrapper _creditImage;

    [Header("システム系")]
    [SerializeField]
    private LoadingCutIn _loadingCutin;

    [Header("データ関係")]
    [SerializeField]
    private ScoreHolder _highScore;
    [SerializeField]
    private ScoreRanking _scoreData;

    private CurrentState _state;

    private Dictionary<CurrentState, Action> _desideKeyPressed;
    private Dictionary<CurrentState, Action> _cancelKeyPressed;
    private Dictionary<CurrentState, Action<Direction>> _dirInputed;

    public override void Initialize()
    {
        _titleMenu.Initialize(CreateButtonFunc());

        _talkTextUI.Initialize();
        _talkTextUI.SetTextAlpha(0.0f);
        _talkFace.Initialize();
        _talkFace.SetImageAlpha(0.0f);
        _talkBG.Initialize();
        _talkBG.SetImageAlpha(0.0f);

        _boardBG.Initialize();
        _boardBG.SetImageAlpha(0.0f);
        _boardFR.Initialize();
        _boardFR.SetImageAlpha(0.0f);

        _difficultyTextUI.Initialize();
        _difficultyTextUI.SetText(_difficultyTexts[(int)_challengeData.ChallengeDifficulty]);
        _difficultyTextUI.SetTextAlpha(0.0f);

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
                    ChangeDifficulty,
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

        _difficultyTextUI.SetTextAlpha(1.0f);
        _boardBG.SetImageAlpha(1.0f);
        _boardFR.SetImageAlpha(1.0f);
        _talkTextUI.SetTextAlpha(1.0f);
        _talkFace.SetImageAlpha(1.0f);
        _talkBG.SetImageAlpha(1.0f);
        _textObj.SetActive(false);

        _boardFR.SetSprite(_inBoardSprites[_titleMenu.CurrentButton]);
        _talkTextUI.SetText(_talkData.TitleTexts[_titleMenu.CurrentButton].Text);
        _talkFace.SetSprite(_talkData.GetFaceSprite(_talkData.TitleTexts[_titleMenu.CurrentButton].FaceType));
    }

    private void CloseMenu()
    {
        if (_titleMenu.DisActive())
            _state = CurrentState.Top;

        _difficultyTextUI.SetTextAlpha(0.0f);
        _boardBG.SetImageAlpha(0.0f);
        _boardFR.SetImageAlpha(0.0f);
        _talkTextUI.SetTextAlpha(0.0f);
        _talkFace.SetImageAlpha(0.0f);
        _talkBG.SetImageAlpha(0.0f);
        _textObj.SetActive(true);
    }

    private void MoveMenu(Direction dir)
    {
        _titleMenu.MoveButton(dir);
        _boardFR.SetSprite(_inBoardSprites[_titleMenu.CurrentButton]);
        _talkTextUI.SetText(_talkData.TitleTexts[_titleMenu.CurrentButton].Text);
        _talkFace.SetSprite(_talkData.GetFaceSprite(_talkData.TitleTexts[_titleMenu.CurrentButton].FaceType));
    }

    private void SelectMenu()
    {
        _titleMenu.SelectButton();
    }

    private void ChangeDifficulty()
    {
        _challengeData.ChallengeDifficulty = (StageDifficulty)
            (((int)_challengeData.ChallengeDifficulty + 1) % 3);
        _difficultyTextUI.SetText(_difficultyTexts[(int)_challengeData.ChallengeDifficulty]);
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
