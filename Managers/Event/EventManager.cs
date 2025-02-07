using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private static readonly Dictionary<string, Action<object>> EventHandlers = new();
    private static readonly Dictionary<string, Action> SimpleEventHandlers = new();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Application.quitting += OnApplicationQuit;
    }

    private static void OnApplicationQuit()
    {
        EventHandlers.Clear();
        SimpleEventHandlers.Clear();
    }

    public static void Publish(string eventName, object eventArgs = null)
    {
        if (EventHandlers.TryGetValue(eventName, out var handler))
        {
            handler?.Invoke(eventArgs);
        }
        
        if (SimpleEventHandlers.TryGetValue(eventName, out var simpleHandler))
        {
            simpleHandler?.Invoke();
        }
    }

    private static void SubscribeInternal(string eventName, Action<object> handler)
    {
        if (!EventHandlers.TryAdd(eventName, handler))
        {
            EventHandlers[eventName] += handler;
        }
    }

    private static void SubscribeInternal(string eventName, Action handler)
    {
        if (!SimpleEventHandlers.TryAdd(eventName, handler))
        {
            SimpleEventHandlers[eventName] += handler;
        }
    }

    private static void UnsubscribeInternal(string eventName, Action<object> handler)
    {
        if (!EventHandlers.ContainsKey(eventName)) return;

        EventHandlers[eventName] -= handler;
        if (EventHandlers[eventName] == null)
        {
            EventHandlers.Remove(eventName);
        }
    }

    private static void UnsubscribeInternal(string eventName, Action handler)
    {
        if (!SimpleEventHandlers.ContainsKey(eventName)) return;

        SimpleEventHandlers[eventName] -= handler;
        if (SimpleEventHandlers[eventName] == null)
        {
            SimpleEventHandlers.Remove(eventName);
        }
    }
    
    public static class Access
    {
        public static void Subscribe(string eventName, Action<object> handler) => SubscribeInternal(eventName, handler);
        public static void Subscribe(string eventName, Action handler) => SubscribeInternal(eventName, handler);
        public static void Unsubscribe(string eventName, Action<object> handler) => UnsubscribeInternal(eventName, handler);
        public static void Unsubscribe(string eventName, Action handler) => UnsubscribeInternal(eventName, handler);
    }
}