using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private class Subscription
    {
        public GameObject Owner;
        public Delegate Handler;
    }

    private static readonly Dictionary<string, List<Subscription>> _nonParamEvents = new();
    private static readonly Dictionary<string, List<Subscription>> _paramEvents = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Application.quitting += () =>
        {
            _nonParamEvents.Clear();
            _paramEvents.Clear();
        };
    }

    #region Subscribe без параметров

    public static void Subscribe(string eventName, GameObject owner, Action handler)
    {
        if (!_nonParamEvents.TryGetValue(eventName, out var list))
        {
            list = new List<Subscription>();
            _nonParamEvents[eventName] = list;
        }
        list.Add(new Subscription { Owner = owner, Handler = handler });
    }

    public static void Trigger(string eventName)
    {
        if (!_nonParamEvents.TryGetValue(eventName, out var list))
            return;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            var sub = list[i];
            if (sub.Owner == null)
            {
                list.RemoveAt(i);
                continue;
            }
            if (!sub.Owner.activeInHierarchy)
                continue;

            if (sub.Handler is Action callback)
                callback.Invoke();
            else
                Debug.LogError($"Event '{eventName}' exists, but delegate type is not Action");
        }

        if (list.Count == 0)
            _nonParamEvents.Remove(eventName);
    }

    #endregion

    #region Subscribe с параметрами

    public static void Subscribe<T>(string eventName, GameObject owner, Action<T> handler)
    {
        if (!_paramEvents.TryGetValue(eventName, out var list))
        {
            list = new List<Subscription>();
            _paramEvents[eventName] = list;
        }
        list.Add(new Subscription { Owner = owner, Handler = handler });
    }

    public static void Trigger<T>(string eventName, T eventArgs)
    {
        if (!_paramEvents.TryGetValue(eventName, out var list))
            return;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            var sub = list[i];
            if (sub.Owner == null)
            {
                list.RemoveAt(i);
                continue;
            }
            if (!sub.Owner.activeInHierarchy)
                continue;

            if (sub.Handler is Action<T> callback)
                callback.Invoke(eventArgs);
            else
                Debug.LogError($"Event '{eventName}' exists, but delegate type is not Action<{typeof(T).Name}>");
        }

        if (list.Count == 0)
            _paramEvents.Remove(eventName);
    }

    #endregion
}
