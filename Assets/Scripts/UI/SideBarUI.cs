using System;
using UnityEngine;
using DG.Tweening;

public class SideBarUI : ButtonBase
{
    [SerializeField]
    private Vector3 _disActivePos;

    [SerializeField]
    private Vector3 _enActivePos;

    [SerializeField]
    private float _activeMoveTime;

    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private string _viewText;

    private RectTransform _transform;

    private Tweener _tweener;

    public override bool IsMoving
        => _tweener == null ? false : _tweener.IsPlaying();

    public override void Initialize(Action func = null)
    {
        _transform = this.GetComponent<RectTransform>();
        _text.Initialize();
        _text.SetText(_viewText);
        _onPressedFunc = func;
    }

    public override bool EnActive()
    {
        if (IsMoving)
            return false;

        _text.SetTextAlpha(1.0f);

        _transform.DOAnchorPos(_enActivePos, _activeMoveTime);

        return true;
    }

    public override bool DisActive()
    {
        if (IsMoving)
            return false;

        _text.SetTextAlpha(0.0f);

        _transform.DOAnchorPos(_disActivePos, _activeMoveTime);

        return true;
    }
}
