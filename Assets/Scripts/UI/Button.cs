using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Button : MonoBehaviour
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
    private string _viewText;

    [ReadOnlySerializeField]
    private int _openWidth = 7;

    [SerializeField]
    private float _openTime;

    private Action _onPressedFunc;

    private Tweener _tweener;

    public bool IsMoving
    {
        get
        {
            if (_tweener == null)
                return false;

            return _tweener.IsPlaying();
        }
    }

    public void Initialize(Action func = null)
    {
        _rightFrame.Initialize();
        _leftFrame.Initialize();
        _text.Initialize();
        _backGround.Initialize();
        _onPressedFunc = func;

        _text.SetTextAlpha(0.0f);
        _text.SetText(_viewText);
    }

    /// <summary>
    /// このボタンが選択された際の処理
    /// </summary>
    public void Selected()
        => _onPressedFunc?.Invoke();

    public bool EnActive()
    {
        if (IsMoving)
            return false;

        OpenMove();

        return true;
    }

    public bool DisActive()
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
        transform.DOScale(scale, _openTime).SetEase(Ease.OutCirc);
        
        transform = _rightFrame.GetComponent<RectTransform>();
        var pos = transform.anchoredPosition;
        pos.x = 25 + 45 * _openWidth;
        transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc);

        transform = _leftFrame.GetComponent<RectTransform>();
        pos = transform.anchoredPosition;
        pos.x = -25 - 45 * _openWidth;
        _tweener = transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc).OnComplete(() => _text.SetTextAlpha(1.0f));
    }

    private void CloseMove()
    {
        _text.SetTextAlpha(0.0f);

        var transform = _backGround.GetComponent<RectTransform>();
        var scale = transform.localScale;
        scale.x = 1;
        transform.DOScale(scale, _openTime).SetEase(Ease.OutCirc);

        transform = _rightFrame.GetComponent<RectTransform>();
        var pos = transform.anchoredPosition;
        pos.x = 25;
        transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc);

        transform = _leftFrame.GetComponent<RectTransform>();
        pos = transform.anchoredPosition;
        pos.x = -25;
        _tweener = transform.DOAnchorPos(pos, _openTime).SetEase(Ease.OutCirc);
    }
}
