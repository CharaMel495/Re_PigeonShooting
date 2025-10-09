using UnityEngine;
using DG.Tweening;

/// <summary>
/// イベントとしてほこりの情報を投げるときのフォーマット
/// </summary>
public record DustValuePackage
{
    public int DustLevel;
    public int DustValue;
}

public class DustMater : MonoBehaviour
{
    [Header("レベル周り")]
    [SerializeField]
    private Sprite[] _levelSprites;
    [SerializeField]
    private Sprite[] _actionSprites;

    [SerializeField]
    private ImageWrapper _dustLevel;
    [SerializeField]
    private ImageWrapper _actionImage;

    [Header("ほこりのバー関係")]

    [SerializeField]
    private ImageWrapper _dustBar;
    private RectTransform _dustBarTransform;

    [SerializeField]
    private Vector3 _maxPos;
    [SerializeField]
    private Vector3 _minPos;
    [SerializeField]
    private float _moveTime;

    private Tweener _barTweener;

    public void Initialize()
    {
        _dustLevel.Initialize();
        _actionImage.Initialize();
        _dustBar.Initialize();
        _dustBarTransform = _dustBar.GetComponent<RectTransform>();
        ResetUI();
        EventDispatcher.Instance.Bind(this);
    }

    public void ResetUI()
    {
        // スタート地点にワープさせる
        _dustBarTransform.anchoredPosition = _minPos;
        _dustLevel.SetSprite(_levelSprites[0]);
    }

    public void UpdateValue(DustValuePackage dustData, int maxValue)
    {
        // もし現在進行中なら
        if (_barTweener.IsActive())
            _barTweener.Kill();

        // 値域制限をかける
        dustData.DustValue = Mathf.Max(0, Mathf.Min(dustData.DustValue, 100));
        var t = Mathf.InverseLerp(0, maxValue, dustData.DustValue);
        // バーの位置を求める
        var barPos = Vector3.Lerp(_minPos, _maxPos, t);
        _barTweener = _dustBarTransform.DOAnchorPos(barPos, _moveTime);

        // レベルに応じて表示を切り替える
        _dustLevel.SetSprite(_levelSprites[Mathf.Max(0, Mathf.Min(dustData.DustLevel, 5))]);
        _actionImage.SetSprite(_actionSprites[Mathf.Max(0, Mathf.Min(dustData.DustLevel, 5))]);
    }
}