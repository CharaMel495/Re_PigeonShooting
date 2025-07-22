using UnityEngine;
using System;

/// <summary>
/// イベントディスパッチャにバインド可能な属性を表す
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CallableEventAttribute : Attribute
{
    /// <summary>
    /// イベント名
    /// </summary>
    public string EventName { get; }
    public CallableEventAttribute(string eventName)
        => EventName = eventName;
}
