using System;
using System.Collections.Generic;

/// <summary>
/// 自作のイベントディスパッチャ
/// </summary>
public class EventDispatcher : SingletonMonoBehaviour<EventDispatcher>
{
    /// <summary>
    /// Dictionalyはハッシュで動くらしいので、stringでもok
    /// </summary>
    private Dictionary<string, Action<object>> _eventTable = new();

    /// <summary>
    /// イベント登録メソッド
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="callback">バインドする関数</param>
    public void Subscribe(string eventName, Action<object> callback)
    {
        if (!_eventTable.ContainsKey(eventName))
            _eventTable[eventName] = delegate { };

        _eventTable[eventName] += callback;
    }

    /// <summary>
    /// イベントを解除するメソッド
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="callback">バインドを解除する関数</param>
    public void Unsubscribe(string eventName, Action<object> callback)
    {
        if (_eventTable.ContainsKey(eventName))
            _eventTable[eventName] -= callback;
    }

    /// <summary>
    /// イベントを発行するメソッド
    /// </summary>
    /// <param name="eventName">発行するイベント</param>
    /// <param name="param">イベント実行に渡すobject</param>
    public void Dispatch(string eventName, object param = null)
    {
        if (_eventTable.ContainsKey(eventName))
            _eventTable[eventName].Invoke(param);
    }
}

public static class EventNames
{
    public static string GetEventName(Events eventType, string eventOwner)
    {
        return eventType switch
        {
            Events.OnHit => $"{eventOwner}OnDamaged",
            Events.OnDead => $"{eventOwner}OnDead",
            Events.OnShotKeyPressed => $"{eventOwner}OnShotKeyPressed",
            Events.OnSubShotKeyPressed => $"{eventOwner}OnSubShotKeyPressed",
            Events.OnBombKeyPressed => $"{eventOwner}OnBombKeyPressed",
            Events.OnMenuKeyPressed => $"{eventOwner}OnMenuKeyPressed",
            Events.OnGameEnd => $"{eventOwner}OnGameEnd",
            _ => null
        };
    }
}

public enum Events
{
    OnHit,
    OnDead,
    OnShotKeyPressed,
    OnSubShotKeyPressed,
    OnBombKeyPressed,
    OnMenuKeyPressed,
    OnGameEnd,
}
