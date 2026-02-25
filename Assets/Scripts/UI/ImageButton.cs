using System;
using UnityEngine;

public class ImageButton : ButtonBase
{
    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private ImageWrapper _imageUI;

    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private string _viewText;

    [SerializeField]
    private Color _imgCol = Color.white;

    public override bool IsMoving => false;

    public override void Initialize(Action func = null)
    {
        _onPressedFunc = func;


        _imageUI.Initialize();
        _imageUI.SetImageAlpha(0.0f);
        _imageUI.SetSprite(_sprite);
        _imageUI.SetImageColor(_imgCol);
        _text.Initialize();
        _text.SetText(_viewText);
        _text.SetTextAlpha(1.0f);
    }

    public override bool EnActive()
    {
        _imageUI.SetImageAlpha(1.0f);
        _text.SetTextAlpha(1.0f);

        return true;
    }

    public override bool DisActive()
    {
        _imageUI.SetImageAlpha(0.8f);
        _text.SetTextAlpha(0.8f);

        return true;
    }
}
