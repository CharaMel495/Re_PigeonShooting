using System;
using UnityEngine;

public class WithIconButton : ButtonBase
{
    [SerializeField]
    private ImageWrapper _iconImage;

    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private string _viewText;

    public override bool IsMoving => false;

    public override void Initialize(Action func = null)
    {
        _onPressedFunc = func;

        _iconImage.Initialize();
        _iconImage.SetImageAlpha(0.0f);
        _text.Initialize();
        _text.SetText(_viewText);
        _text.SetTextAlpha(1.0f);
    }

    public override bool EnActive()
    {
        _iconImage.SetImageAlpha(1.0f);

        return true;
    }

    public override bool DisActive()
    {
        _iconImage.SetImageAlpha(0.0f);

        return true;
    }
}
