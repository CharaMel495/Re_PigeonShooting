using UnityEngine;

/// <summary>
/// ウェーブイベントを保持して実行や
/// 経過時間に応じて破棄などを行うクラス
/// </summary>
public class WaveEventHolder
{
    private WaveEvent _holdEvent;
    private float _elapsedTime;
    private float _totalTime;

    public bool IsEnded
        => _totalTime >= _holdEvent.Duration;

    private WaveEventHolder() { }

    public WaveEventHolder(WaveEvent throwEvent)
    {
        _holdEvent = throwEvent;
        _elapsedTime = 0.0f;
        _totalTime = 0.0f;
        _holdEvent.Execute();
    }

    public void Update()
    {
        if (IsEnded)
            return;

        _elapsedTime += Time.fixedDeltaTime;
        _totalTime += Time.fixedDeltaTime;

        if (_elapsedTime < _holdEvent.Interval)
            return;

        // 設定された時間毎にイベントを起こす
        _elapsedTime = 0.0f;
        _holdEvent.Execute();
    }
}
