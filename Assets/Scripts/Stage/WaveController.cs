using EnemyEnums;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UIElements;

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

        // 敵を全消去する必要があれば行う
        if (invokeEventOption.IsClearEnemys)
            EventDispatcher.Instance.Dispatch("ClearAllEnemy");

        if (invokeEventOption.EventID < 0)
            EventDispatcher.Instance.Dispatch("BossEvent", GetBossSpawnPosition());
        else
        {
            var invokeEvent = _events.Single(item => item.ID == invokeEventOption.EventID);

            _holdEvents.Add(new(invokeEvent));
        }
        //_elapsedTime = 0;

        Vector3 GetBossSpawnPosition()
        {
            var playerPos = PlayerManager.Instance.Player.GetPosition();

            float angle = UnityEngine.Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;

            return playerPos + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * 24f;
        }
    }

    private void FlushHoldEvents()
    {
        _holdEvents.RemoveAll(e => e.IsEnded);
    }
}
