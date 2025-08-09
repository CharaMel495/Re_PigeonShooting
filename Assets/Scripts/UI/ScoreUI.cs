using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextWrapper _scoreText;

    [SerializeField]
    private TextWrapper _highText;

    [SerializeField]
    private ScoreHolder _highScore;

    private int _finalScore;

    private int _viewingScore;

    public void Initialize(int score)
    {
        _finalScore = score;
        _viewingScore = 0;
        _scoreText.Initialize();
        _highText.Initialize();

        if (_highScore.Score < _finalScore)
            _highScore.Score = _finalScore;

        _highText.SetText($"{_highScore.Score:D8}pts");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_finalScore != _viewingScore)
        {
            ++_viewingScore;
            _scoreText.SetText($"{_viewingScore:D8}pts");
        }
    }

    public void Skip()
    {
        _viewingScore = _finalScore;
        _scoreText.SetText($"{_viewingScore:D8}pts");
    }
}
