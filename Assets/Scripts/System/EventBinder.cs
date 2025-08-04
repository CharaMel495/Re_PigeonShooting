using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// [CallableEvent] を使ったメソッド自動登録/解除を担当するクラス
/// </summary>
public class EventBinder
{
    /// <summary>
    /// イベント発火クラス
    /// </summary>
    private readonly EventDispatcher _dispatcher;

    /// <summary>
    /// コンストラクタでディスパッチャを知る
    /// </summary>
    public EventBinder(EventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// 対象オブジェクトの [CallableEvent] メソッドを全て登録
    /// </summary>
    public void Bind(object owner, string prefix = "")
    {
        // 全てのインスタンスメソッドを取得
        var methods = owner.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var method in methods)
        {
            // もしイベント呼び出し可能な属性がついてるなら続行
            var attr = method.GetCustomAttribute<CallableEventAttribute>();
            if (attr == null) 
                continue;

            // メソッドの情報を取得
            var parameters = method.GetParameters();
            // 第一引数の型がobject型ならイベントとしてバインドする
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object))
            {
                try
                {
                    var action = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), owner, method);
                    _dispatcher.Subscribe($"{prefix}{attr.EventName}", action);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBinder] {method.Name} のイベント登録失敗 ({ex.Message})");
                }
            }
            else
            {
                Debug.LogWarning($"[EventBinder] {method.Name} は Action<object> と互換性がありません。");
            }
        }
    }

    /// <summary>
    /// 対象オブジェクトの [CallableEvent] メソッドを全て解除
    /// </summary>
    public void Unbind(object owner, string prefix = "")
    {
        // 全てのインスタンスメソッドを取得
        var methods = owner.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var method in methods)
        {
            // イベント呼び出し可能属性出なければ登録されないので弾く
            var attr = method.GetCustomAttribute<CallableEventAttribute>();
            if (attr == null) 
                continue;

            try
            {
                var action = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), owner, method);
                _dispatcher.Unsubscribe($"{attr.EventName}{prefix}", action);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[EventBinder] {method.Name} のイベント解除失敗 ({ex.Message})");
            }
        }
    }
}
