using System;
using UnityEngine;

public class TileButton : ButtonBase
{

    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private string _viewText;

    [Header("前面画像用項目")]
    [SerializeField]
    private ImageWrapper _frontImage;
    [SerializeField]
    private float _maxFRSize;
    [SerializeField]
    private float _minFRSize;
    [SerializeField]
    private float _scalingTimeFR;
    [SerializeField]
    private Color _frontCol = Color.white;

    [Header("背景画像用項目")]
    [SerializeField]
    private ImageWrapper _backGroundImage;
    [SerializeField]
    private float _maxBGSize;
    [SerializeField]
    private float _minBGSize;
    [SerializeField]
    private float _scalingTimeBG;

    [SerializeField]
    private Color _backCol = Color.white;

    private bool _isMoving;
    public override bool IsMoving => _isMoving;

    private Timer _timer;
    // アクティブになった後、他のボタンへ移動可能になるまで、どれだけ待つか
    private readonly float _ENACTIVEDWAITTIME = 0.5f;
    private Durator _durator;
    private int _duratorTaskID;


    private Vector3 _maxCacheFR;
    private Vector3 _minCacheFR;
    private Vector3 _maxCacheBG;
    private Vector3 _minCacheBG;

    public override void Initialize(Action func = null)
    {
        _onPressedFunc = func;

        _timer = new();
        _durator = new();

        _frontImage.Initialize();
        _frontImage.SetImageAlpha(0.0f);
        _frontImage.SetImageColor(_frontCol);
        _backGroundImage.Initialize();
        _backGroundImage.SetImageAlpha(0.0f);
        _backGroundImage.SetImageColor(_backCol);
        _text.Initialize();
        _text.SetText(_viewText);
        _text.SetTextAlpha(1.0f);

        _isMoving = false;

        _maxCacheFR = new(_maxFRSize, _maxFRSize, _maxFRSize);
        _minCacheBG = new(_minFRSize, _minFRSize, _minFRSize);
        _maxCacheBG = new(_maxBGSize, _maxBGSize, _maxBGSize);
        _minCacheBG = new(_minBGSize, _minBGSize, _minBGSize);
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

        return true;
    }

    public override bool DisActive()
    {
        _backGroundImage.SetImageAlpha(0.0f);

        _durator.CanncellTask(_duratorTaskID);

        return true;
    }

    public override void Selected()
    {
        if (IsMoving)
            return;

        base.Selected();

        StartScaleDown();
    }

    private void StartScaleDown()
    {
        _isMoving = true;
        _backGroundImage.transform.localScale = _maxCacheFR;
        _durator.CreateTask(ScaleDownFrontImage, StartScaleUp, _scalingTimeFR);
    }

    private void StartScaleUp()
    {
        _backGroundImage.transform.localScale = _minCacheFR;
        _durator.CreateTask(ScaleUpFrontImage, 
            OnSelectedAnimEnd, _scalingTimeFR);
    }

    private void OnSelectedAnimEnd()
    {
        _backGroundImage.transform.localScale = _maxCacheFR;
        _isMoving = false;
    }

    private void ScaleDownFrontImage(float elapsedTime, float endTime)
    {
        var t = Mathf.InverseLerp(0, endTime, elapsedTime);
        var scale = Mathf.Lerp(_maxFRSize, _minFRSize, t);
        var currentFRScale = _backGroundImage.transform.localScale;
        currentFRScale.x = scale;
        currentFRScale.y = scale;
        currentFRScale.z = scale;
        _frontImage.transform.localScale = currentFRScale;
    }

    private void ScaleUpFrontImage(float elapsedTime, float endTime)
    {
        var t = Mathf.InverseLerp(0, endTime, elapsedTime);
        var scale = Mathf.Lerp(_minFRSize, _maxFRSize, t);
        var currentFRScale = _backGroundImage.transform.localScale;
        currentFRScale.x = scale;
        currentFRScale.y = scale;
        currentFRScale.z = scale;
        _frontImage.transform.localScale = currentFRScale;
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
        _backGroundImage.transform.localScale = _minCacheBG;
        _duratorTaskID = _durator.CreateTask(UpdateBackGroundImage, RestartBackGround, _scalingTimeBG);
    }
}
