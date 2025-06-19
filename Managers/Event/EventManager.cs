using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private static readonly Dictionary<string, Delegate> EventHandlers = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Application.quitting += OnApplicationQuit;
    }

    private static void OnApplicationQuit()
    {
        EventHandlers.Clear();
    }

    #region Subscribe with parameters

    public static void Subscribe<T>(string eventName, Action<T> handler)
    {
        if (EventHandlers.TryGetValue(eventName, out var existing))
        {
            EventHandlers[eventName] = Delegate.Combine(existing, handler);
        }
        else
        {
            EventHandlers.Add(eventName, handler);
        }
    }

    public static void Unsubscribe<T>(string eventName, Action<T> handler)
    {
        if (EventHandlers.TryGetValue(eventName, out var existing))
        {
            var currentDel = Delegate.Remove(existing, handler);
            if (currentDel == null)
                EventHandlers.Remove(eventName);
            else
                EventHandlers[eventName] = currentDel;
        }
    }

    public static void Publish<T>(string eventName, T eventArgs)
    {
        if (EventHandlers.TryGetValue(eventName, out var del))
        {
            if (del is Action<T> callback)
            {
                callback.Invoke(eventArgs);
            }
            else
            {
                Debug.LogError($"Событие '{eventName}' существует, но тип делегата не соответствует Action<{typeof(T).Name}>");
            }
        }
    }

    #endregion

    #region Subscribe without parameters

    public static void Subscribe(string eventName, Action handler)
    {
        if (EventHandlers.TryGetValue(eventName, out var existing))
        {
            EventHandlers[eventName] = Delegate.Combine(existing, handler);
        }
        else
        {
            EventHandlers.Add(eventName, handler);
        }
    }

    public static void Unsubscribe(string eventName, Action handler)
    {
        if (EventHandlers.TryGetValue(eventName, out var existing))
        {
            var currentDel = Delegate.Remove(existing, handler);
            if (currentDel == null)
                EventHandlers.Remove(eventName);
            else
                EventHandlers[eventName] = currentDel;
        }
    }

    public static void Publish(string eventName)
    {
        if (EventHandlers.TryGetValue(eventName, out var del))
        {
            if (del is Action callback)
            {
                callback.Invoke();
            }
            else
            {
                Debug.LogError($"Событие '{eventName}' существует, но тип делегата не соответствует Action");
            }
        }
    }

    #endregion
}
