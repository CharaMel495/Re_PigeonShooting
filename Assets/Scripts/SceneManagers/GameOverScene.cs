using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : SceneManagerBase<GameOverScene>
{
    [SerializeField]
    private LoadingCutIn _loadingCutin;

    [SerializeField]
    private ButtonMenuController _gameOverMenu;

    [SerializeField]
    private ScoreHolder _scoreHolder;

    [SerializeField]
    private ScoreRanking _scoreRankingData;

    [SerializeField]
    private ScoreUI _scoreUI;

    public override void Initialize()
    {
        CRISoundManager.Instance.PlayBGM(BGM.Ranking);

        _gameOverMenu.Initialize(CreateButtonFunc());

        _gameOverMenu.EnActive();

        _loadingCutin.ExitCutin();

        _scoreRankingData.AddScore(_scoreHolder);
        _scoreUI.Initialize(_scoreHolder.Score, _scoreRankingData.GetHighScore());

        InputManager.Instance.ChangeInputHandler(InputHandler.UI);

        //CRISoundManager.Instance.PlayVoice(Voice.Result_Voice);

        Action[] CreateButtonFunc()
        {
            return new Action[]
                {
                    () => _loadingCutin.EnterCutin(() => SceneManager.LoadScene("MainGame")),
                    () => _loadingCutin.EnterCutin(() => SceneManager.LoadScene("Title"))
                };
        }
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.PauseKey, InputHandler.UI))
            _scoreUI.Skip();

        if (InputManager.CheckKey(InputManager.DesideKey, InputHandler.UI))
            _gameOverMenu.SelectButton();

        Direction inputDir = InputManager.CheckInputDirection(InputHandler.UI);

        if (inputDir == Direction.Down)
            _gameOverMenu.MoveButton(true);

        if (inputDir == Direction.Up)
            _gameOverMenu.MoveButton(false);
    }
}
