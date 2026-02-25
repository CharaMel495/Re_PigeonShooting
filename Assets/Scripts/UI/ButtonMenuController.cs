using System;
using UnityEngine;

public class ButtonMenuController : MonoBehaviour
{
    [SerializeField]
    private ImageWrapper _panel;

    [SerializeField]
    private ButtonBase[] _buttons;

    [SerializeField]
    private Color _panelColor;

    private Durator _durator;

    public bool IsMoving
    { get; private set; }

    public int CurrentButton
    { get; private set; }

    public void Initialize(Action[] buttonFunc)
    {
        _panel.Initialize();
        _panel.SetImageColor(Color.clear);

        if (_buttons.Length == buttonFunc.Length)
        {
            for (int i = 0; i < _buttons.Length; ++i)
            {
                _buttons[i].Initialize(buttonFunc[i]);
                _buttons[i].gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var button in _buttons)
            {
                button.Initialize();
                button.gameObject.SetActive(false);
            }
        }

        _durator = new();
        _durator.Initialize();

        IsMoving = false;

        CurrentButton = 0;
    }

    private void Update()
        => _durator.Update();

    public void MoveButton(Direction dir)
    {
        int nextIdx = -1;
        var currentButton = _buttons[CurrentButton];

        switch (dir)
        {
            case Direction.Up:
                nextIdx = currentButton.MoveRuler.UpButtonNum;
                break;
            case Direction.Down:
                nextIdx = currentButton.MoveRuler.DownButtonNum;
                break;
            case Direction.Left:
                nextIdx = currentButton.MoveRuler.LeftButtonNum;
                break;
            case Direction.Right:
                nextIdx = currentButton.MoveRuler.RightButtonNum;
                break;
            default:
                return;
        }

        var nextButton = _buttons[nextIdx];

        if (currentButton.IsMoving || nextButton.IsMoving)
            return;

        currentButton.DisActive();
        nextButton.EnActive();
        CurrentButton = nextIdx;
    }

    public void MoveButton(bool isDown)
    {
        int nextIdx = CurrentButton + (isDown ? 1 : -1);
        if (nextIdx < 0 || nextIdx >= _buttons.Length)
            return;

        var currentButton = _buttons[CurrentButton];
        var nextButton = _buttons[nextIdx];

        if (currentButton.IsMoving || nextButton.IsMoving)
            return;

        currentButton.DisActive();
        nextButton.EnActive();
        CurrentButton = nextIdx;
    }

    public void MoveButton(int nextIdx)
    {
        if (nextIdx < 0 || nextIdx >= _buttons.Length)
            return;

        if (nextIdx == CurrentButton)
            return;

        var currentButton = _buttons[CurrentButton];
        var nextButton = _buttons[nextIdx];

        if (currentButton.IsMoving || nextButton.IsMoving)
            return;

        currentButton.DisActive();
        nextButton.EnActive();
        CurrentButton = nextIdx;
    }

    public bool EnActive(int openButtonIdx = 0)
    {
        if (IsMoving)
            return false;

        IsMoving = true;

        _durator.CreateTask(AppearPanel, () => AppearButton(openButtonIdx), 0.1f, onUnscaledTime: true);

        return true;
    }

    public bool DisActive()
    {
        if (IsMoving)
            return false;

        IsMoving = true;

        DisAppearButton();

        _durator.CreateTask(DisAppearPanel, null, 0.1f, onUnscaledTime: true);

        return true;
    }

    private void AppearPanel(float _elapsedTime, float _endTime)
    {
        var ratio = _elapsedTime / _endTime;
        _panel.SetImageColor(Color.Lerp(Color.clear, _panelColor, ratio));
    }

    private void DisAppearPanel(float _elapsedTime, float _endTime)
    {
        var ratio = _elapsedTime / _endTime;
        _panel.SetImageColor(Color.Lerp(_panelColor, Color.clear, ratio));
    }

    private void AppearButton(int openButtonIdx = 0)
    {
        foreach (var button in _buttons)
            button.gameObject.SetActive(true);

        _buttons[openButtonIdx].EnActive();

        IsMoving = false;
    }

    private void DisAppearButton()
    {
        foreach (var button in _buttons)
        {
            button.DisActive();
            button.gameObject.SetActive(false);
        }
        IsMoving = false;
    }

    public void SelectButton()
    {
        var button = _buttons[CurrentButton];

        if (button.IsMoving)
            return;

        button.Selected();
    }
}
