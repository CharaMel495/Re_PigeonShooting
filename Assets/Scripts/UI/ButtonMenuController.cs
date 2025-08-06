using System;
using UnityEngine;

public class ButtonMenuController : MonoBehaviour
{
    [SerializeField]
    private ImageWrapper _panel;

    [SerializeField]
    private Button[] _buttons;

    [SerializeField]
    private Color _panelColor;

    private Durator _durator;

    public bool IsMoving
    { get; private set; }

    private int _currentButton;

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

        _currentButton = 0;
    }

    private void FixedUpdate()
        => _durator.Update();

    public void MoveButton(bool isDown)
    {
        int nextIdx = _currentButton + (isDown ? 1 : -1);
        if (nextIdx < 0 || nextIdx >= _buttons.Length)
            return;

        var currentButton = _buttons[_currentButton];
        var nextButton = _buttons[nextIdx];

        if (currentButton.IsMoving || nextButton.IsMoving)
            return;

        currentButton.DisActive();
        nextButton.EnActive();
        _currentButton = nextIdx;
    }

    public bool EnActive()
    {
        if (IsMoving)
            return false;

        IsMoving = true;

        _durator.CreateTask(AppearPanel, AppearButton, 0.1f);

        return true;
    }

    public bool DisActive()
    {
        if (IsMoving)
            return false;

        IsMoving = true;

        DisAppearButton();

        _durator.CreateTask(DisAppearPanel, null, 0.1f);

        _currentButton = 0;

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

    private void AppearButton()
    {
        foreach (var button in _buttons)
            button.gameObject.SetActive(true);

        _buttons[0].EnActive();

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
        var button = _buttons[_currentButton];

        if (button.IsMoving)
            return;

        button.Selected();
    }
}
