using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : SceneManagerBase<GameOverScene>
{
    [SerializeField]
    private LoadingCutIn _loadingCutin;

    [SerializeField]
    private ButtonMenuController _gameOverMenu;

    public override void Initialize()
    {
        CRISoundManager.Instance.PlayBGM(BGM.Ranking);

        _gameOverMenu.Initialize(CreateButtonFunc());

        _gameOverMenu.EnActive();

        _loadingCutin.ExitCutin();

        Action[] CreateButtonFunc()
        {
            return new Action[]
                {
                    () => SceneManager.LoadScene("Title")
                };
        }
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.DesideKey, InputHandler.Player))
            _gameOverMenu.SelectButton();
    }
}
