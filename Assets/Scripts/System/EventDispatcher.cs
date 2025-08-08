using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 自作のイベントディスパッチャ
/// </summary>
public class EventDispatcher : SingletonMonoBehaviour<EventDispatcher>
{
    /// <summary>
    /// Dictionalyはハッシュで動くらしいので、stringでもok
    /// </summary>
    private Dictionary<string, Action<object>> _eventTable = new();

    private EventBinder _binder;

    public void Initialize()
    {
        _binder = new(Instance);
    }

    public void BulkResisterMethod(MethodInfo[] methods, object owner)
    {
        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<CallableEventAttribute>();
            if (attr == null) 
                continue;

            // メソッドシグネチャ確認
            var parameters = method.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object))
            {
                try
                {
                    var action = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), owner, method);
                    this.Subscribe(attr.EventName, action);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"イベント登録失敗: {method.Name} ({ex.Message})");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Method {method.Name} は Action<object> と互換性がありません。");
            }
        }
    }


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

    public void Bind(object owner, string prefix = "")
        => _binder.Bind(owner, prefix);

    public void Unbind(object owner, string prefix = "")
        => _binder.Unbind(owner, prefix);
}

public static class EventNames
{
    public static string GetEventName(Events eventType, string eventOwner)
    {
        return eventType switch
        {
            Events.OnTriggerEnter => $"{eventOwner}OnTriggerEnter",
            Events.OnTriggerStay => $"{eventOwner}OnTriggerStay",
            Events.OnTriggerExit => $"{eventOwner}OnTriggerExit",
            Events.OnHit => $"{eventOwner}OnHit",
            Events.OnDead => $"{eventOwner}OnDead",
            Events.OnSmashed => $"{eventOwner}OnSmashed",
            Events.OnShotKeyPressed => $"{eventOwner}OnShotKeyPressed",
            Events.OnVacuumKeyPressed => $"{eventOwner}OnVacuumKeyPressed",
            Events.OnVacuumKeyReleased => $"{eventOwner}OnVacuumKeyReleased",
            Events.OnDashKeyPressed => $"{eventOwner}OnDashKeyPressed",
            Events.OnMenuKeyPressed => $"{eventOwner}OnMenuKeyPressed",
            Events.OnGameEnd => $"{eventOwner}OnGameEnd",
            Events.OnAirBasterKeyPressed => $"{eventOwner}OnAirBasterKeyPressed",
            Events.OnGetItem => $"{eventOwner}OnGetItem",
            _ => null
        };
    }
}

public enum Events
{
    OnTriggerEnter,
    OnTriggerStay,
    OnTriggerExit,
    OnHit,
    OnDead,
    OnSmashed,
    OnShotKeyPressed,
    OnVacuumKeyPressed,
    OnVacuumKeyReleased,
    OnDashKeyPressed,
    OnAirBasterKeyPressed,
    OnMenuKeyPressed,
    OnGameEnd,
    OnGetItem,
}
