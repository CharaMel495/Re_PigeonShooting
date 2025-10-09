using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextWrapper _scoreText;

    [SerializeField]
    private TextWrapper _highText;

    private int _finalScore;

    private int _viewingScore;

    public void Initialize(int score, int highScore)
    {
        _finalScore = score;
        _viewingScore = 0;
        _scoreText.Initialize();
        _highText.Initialize();

        _highText.SetText($"{highScore:D8}pts");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_finalScore != _viewingScore)
        {
            _viewingScore = Mathf.Min(_viewingScore + 1000, _finalScore);
            _scoreText.SetText($"{_viewingScore:D8}pts");
        }
    }

    public void Skip()
    {
        _viewingScore = _finalScore;
        _scoreText.SetText($"{_viewingScore:D8}pts");
    }
}
