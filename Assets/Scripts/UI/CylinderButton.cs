using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CylinderButton : ButtonBase
{
    [SerializeField]
    private ImageWrapper _rightFrame;

    [SerializeField]
    private ImageWrapper _leftFrame;

    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private ImageWrapper _backGround;

    [SerializeField]
    [TextArea]
    private string _viewText;

    [ReadOnlySerializeField]
    private int _openWidth = 7;

    [SerializeField]
    private float _openTime;

    private Tweener _tweener;

    public override bool IsMoving
    {
        get
        {
            if (_tweener == null)
                return false;

            return _tweener.IsPlaying();
        }
    }

    public override void Initialize(Action func = null)
    {
        _rightFrame.Initialize();
        _leftFrame.Initialize();
        _text.Initialize();
        _backGround.Initialize();
        _onPressedFunc = func;

        _text.SetTextAlpha(0.0f);
        _text.SetText(_viewText);
    }

    public override bool EnActive()
    {
        if (IsMoving)
            return false;

        OpenMove();

        CRISoundManager.Instance.PlaySE(SFX.NormalButton);

        return true;
    }

    public override bool DisActive()
    {
        if (IsMoving)
            return false;

        CloseMove();

        return true;
    }

    private void OpenMove()
    {
        var transform = _backGround.GetComponent<RectTransform>();
        var scale = transform.localScale;
        scale.x = _openWidth;
        transform.DOScale(scale, _openTime).SetEase(Ease.OutCirc).SetUpdate(true);
        
        transform = _rightFrame.GetComponent<RectTransform>();
        var pos = transform.anchoredPosition;
        pos.x = 25 + 45 * _openWidth;
        transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc).SetUpdate(true);

        transform = _leftFrame.GetComponent<RectTransform>();
        pos = transform.anchoredPosition;
        pos.x = -25 - 45 * _openWidth;
        _tweener = transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc).OnComplete(() => _text.SetTextAlpha(1.0f)).SetUpdate(true);
    }

    private void CloseMove()
    {
        _text.SetTextAlpha(0.0f);

        var transform = _backGround.GetComponent<RectTransform>();
        var scale = transform.localScale;
        scale.x = 1;
        transform.DOScale(scale, _openTime).SetEase(Ease.OutCirc).SetUpdate(true);

        transform = _rightFrame.GetComponent<RectTransform>();
        var pos = transform.anchoredPosition;
        pos.x = 25;
        transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc).SetUpdate(true);

        transform = _leftFrame.GetComponent<RectTransform>();
        pos = transform.anchoredPosition;
        pos.x = -25;
        _tweener = transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc).SetUpdate(true);
    }
}
