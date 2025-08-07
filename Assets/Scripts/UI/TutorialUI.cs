using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField]
    private ImageWrapper _backGround;

    [SerializeField]
    private ImageWrapper _icon;

    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private GameObject _nextObj;

    [SerializeField]
    private TextWrapper _nextText;

    [SerializeField]
    private float _typeSpeed;

    [SerializeField]
    private Sprite[] _iconFaces;

    private Timer _timer;

    public bool IsPlaying
    { get; private set; }

    private TutorialTextSet[] _tutorialTexts;
    private int _textIdx;

    private TutorialTextSet _currentTypeText;
    private int _typeIndex;

    public bool IsEndTutorial
        => _textIdx >= _tutorialTexts.Length;

    public void Initialize(TutorialTextSet[] tutorialTexts)
    {
        _backGround.Initialize();
        _icon.Initialize();
        _text.Initialize();
        _timer = new();
        _timer.Initialize();
        _nextText.Initialize();
        IsPlaying = false;
        _tutorialTexts = tutorialTexts;
        _textIdx = 0;
        _typeIndex = 0;
    }

    public void ClearnUp()
    {
        _text.ClearText();
    }

    private void FixedUpdate()
    {
        _timer.Update();
    }

    public bool PlayNext()
    {
        if (IsEndTutorial)
        {
            EventDispatcher.Instance.Dispatch("EndTutorial");
            return false;
        }

        if(IsPlaying)
            return false;

        _currentTypeText = _tutorialTexts[_textIdx];
        _nextObj.SetActive(false);
        _text.ClearText();
        _icon.SetSprite(_iconFaces[(int)_currentTypeText.FaceType]);
        _timer.CreateTask(TypeText, _typeSpeed);
        IsPlaying = true;
        _typeIndex = 0;

        return true;
    }

    private void TypeText()
    {
        _text.AddText(_currentTypeText.Text[_typeIndex]);

        ++_typeIndex;

        if (_typeIndex >= _currentTypeText.Text.Length)
            EndTyping();
        else if (_currentTypeText.Text[_typeIndex] == '#')
        {
            EndTyping();
            _nextObj.SetActive(false);
            EventDispatcher.Instance.Dispatch("EnterPlayingMode");
        }
        else
        _timer.CreateTask(TypeText, _typeSpeed);
    }

    private void EndTyping()
    {
        IsPlaying = false;
        ++_textIdx;

        if (!IsEndTutorial)
            _nextText.SetText("Next");
        else
            _nextText.SetText("End");

        _nextObj.SetActive(true);
    }
}
