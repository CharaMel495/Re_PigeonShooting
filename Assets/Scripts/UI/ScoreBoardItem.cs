using UnityEngine;

public class ScoreBoardItem : MonoBehaviour
{
    [SerializeField]
    private TextWrapper _scoreText;

    [SerializeField]
    private TextWrapper _rankText;

    [SerializeField]
    private ImageWrapper _crownImage;

    [SerializeField]
    private Sprite[] _crownSprites;

    public void Initialize(int ranking, int score)
    {
        _scoreText.Initialize();
        _rankText.Initialize();
        _crownImage.Initialize();
        _scoreText.SetText($"{score:D8}pts");
        _rankText.SetText($"{ranking + 1}");
        if (ranking < 3)
            _crownImage.SetSprite(_crownSprites[ranking]);
        else
            _crownImage.SetSprite(_crownSprites[3]);
    }
}
