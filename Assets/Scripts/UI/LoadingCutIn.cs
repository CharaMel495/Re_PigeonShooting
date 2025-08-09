using UnityEngine;
using DG.Tweening;
using System;

public class LoadingCutIn : MonoBehaviour
{
    [SerializeField]
    private RectTransform _cutinTransform;

    [SerializeField]
    private Vector3 _startPos;

    [SerializeField]
    private Vector3 _endPos;

    [SerializeField]
    private float _moveTime;

    public void EnterCutin(Action onEndCutin = null)
    {
        _cutinTransform.anchoredPosition = _startPos;

        _cutinTransform.DOAnchorPos(Vector3.zero, _moveTime).SetEase(Ease.InOutQuart).OnComplete(() => onEndCutin?.Invoke()).SetUpdate(true);

        CRISoundManager.Instance.PlaySE(SFX.Cutin);
    }

    public void ExitCutin(Action onEndCutin = null)
    {
        _cutinTransform.DOAnchorPos(_endPos, _moveTime).SetEase(Ease.Linear).OnComplete(() => onEndCutin?.Invoke()).SetUpdate(true);

        CRISoundManager.Instance.PlaySE(SFX.Cutin);
    }
}
