using System;
using UnityEngine;

public class TileButton : ButtonBase
{
    [SerializeField]
    private ImageWrapper _backGroundImage;

    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private string _viewText;

    [SerializeField]
    private float _maxBGSize;
    [SerializeField]
    private float _minBGSize;
    [SerializeField]
    private float _scalingTime;

    [SerializeField]
    private Color _backCol = Color.white;

    private bool _isMoving;
    public override bool IsMoving => _isMoving;

    private Timer _timer;
    // アクティブになった後、他のボタンへ移動可能になるまで、どれだけ待つか
    private readonly float _ENACTIVEDWAITTIME = 0.5f;
    private Durator _durator;
    private int _duratorTaskID;

    private Vector3 _maxCache;
    private Vector3 _minCache;

    public override void Initialize(Action func = null)
    {
        _onPressedFunc = func;

        _timer = new();
        _durator = new();

        _backGroundImage.Initialize();
        _backGroundImage.SetImageAlpha(0.0f);
        _backGroundImage.SetImageColor(_backCol);
        _text.Initialize();
        _text.SetText(_viewText);
        _text.SetTextAlpha(1.0f);

        _isMoving = false;

        _maxCache = new(_maxBGSize, _maxBGSize, _maxBGSize);
        _minCache = new(_minBGSize, _minBGSize, _minBGSize);
    }

    private void FixedUpdate()
    {
        _timer.Update();
        _durator.Update();
    }

    public override bool EnActive()
    {
        _backGroundImage.SetImageAlpha(1.0f);

        RestartBackGround();

        //_isMoving = true;
        //_timer.CreateTask(() => _isMoving = false, _ENACTIVEDWAITTIME);

        return true;
    }

    public override bool DisActive()
    {
        _backGroundImage.SetImageAlpha(0.0f);

        _durator.CanncellTask(_duratorTaskID);

        return true;
    }

    private void UpdateBackGroundImage(float elapsedTime, float endTime)
    {
        var t = Mathf.InverseLerp(0, endTime, elapsedTime);
        var scale = Mathf.Lerp(_minBGSize, _maxBGSize, t);
        var currentBGScale = _backGroundImage.transform.localScale;
        currentBGScale.x = scale;
        currentBGScale.y = scale;
        currentBGScale.z = scale;
        _backGroundImage.transform.localScale = currentBGScale;
        _backGroundImage.SetImageAlpha(1 - t);
    }

    private void RestartBackGround()
    {
        _backGroundImage.SetImageAlpha(0.0f);
        _backGroundImage.transform.localScale = _minCache;
        _duratorTaskID = _durator.CreateTask(UpdateBackGroundImage, RestartBackGround, _scalingTime);
    }
}
