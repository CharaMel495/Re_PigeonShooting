using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 雑魚敵のウェーブの管理を行うクラス
/// </summary>
public class WaveController
{
    /// <summary>
    /// 一定時間ごとに起きるウェーブイベント
    /// </summary>
    private List<WaveEvent> _events;

    private List<WaveEventHolder> _holdEvents;

    private Queue<WaveTimeTable> _eventQueue;

    // 経過時間
    private float _elapsedTime;

    public WaveController()
    {
        _events = new();
        _holdEvents = new();
        _eventQueue = new();

        // 必要なデータをAddressables経由で取得する
        var tableAsset = Addressables.LoadAssetAsync<WaveEventTableAsset>(SummarizeResourceDirectory.WAVEEVENTTABLEASSET_PATH).WaitForCompletion();
        var timeTableAsset = Addressables.LoadAssetAsync<WaveTimeTableAsset>(SummarizeResourceDirectory.WAVETIMETABLEASSET_PATH).WaitForCompletion();

        // ウェーブイベントの登録
        foreach (var eventData in tableAsset.WaveEventTable)
            _events.Add(new(eventData));

        // イベントのキューを作成
        var sorted = new List<WaveTimeTable>();
        foreach (var item in timeTableAsset.WaveTimeTable)
            sorted.Add(new WaveTimeTable
            {
                ID = item.ID,
                StartTime = item.StartTime,
                EventID = item.EventID,
                IsClearEnemys = item.IsClearEnemys
            });

        sorted.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

        foreach (var entry in sorted)
            _eventQueue.Enqueue(entry);

        _elapsedTime = 0f;
    }

    public void Update()
    {
        _elapsedTime += Time.fixedDeltaTime;

        foreach (var e in _holdEvents.ToArray())
            e.Update();

        if (_eventQueue.Count < 1)
            return;

        if (_elapsedTime < _eventQueue.Peek().StartTime)
            return;

        StartNextWaveEvent();
        FlushHoldEvents();
    }

    private void StartNextWaveEvent()
    {
        var invokeEventOption = _eventQueue.Dequeue();

        var invokeEvent = _events.Single(item => item.ID == invokeEventOption.EventID);

        _holdEvents.Add(new(invokeEvent));

        //_elapsedTime = 0;
    }

    private void FlushHoldEvents()
    {
        _holdEvents.RemoveAll(e => e.IsEnded);
    }
}
