using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Search;

/// <summary>
/// 自作のイベントディスパッチャ
/// </summary>
public class EventDispatcher : SingletonMonoBehaviour<EventDispatcher>
{
    /// <summary>
    /// Dictionalyはハッシュで動くらしいので、stringでもok
    /// </summary>
    private Dictionary<string, Action<object>> _eventTable = new();

    private EventBinder _binder = new(Instance);

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
            Events.OnHit => $"{eventOwner}OnDamaged",
            Events.OnDead => $"{eventOwner}OnDead",
            Events.OnSmashed => $"{eventOwner}OnSmashed",
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
    OnSmashed,
    OnShotKeyPressed,
    OnSubShotKeyPressed,
    OnBombKeyPressed,
    OnMenuKeyPressed,
    OnGameEnd,
}
