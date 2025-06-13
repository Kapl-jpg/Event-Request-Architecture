using System;
using System.Collections.Generic;
using UnityEngine;

namespace ERA
{
    public static class EventManager
    {
        private static readonly Dictionary<string, List<EventBinding>> _noArgEvents = new();
        private static readonly Dictionary<string, List<EventBinding>> _oneArgEvents = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Application.quitting += OnApplicationQuit;
        }

        private static void OnApplicationQuit()
        {
            _noArgEvents.Clear();
            _oneArgEvents.Clear();
        }

        public static void AddEvent(string eventName, GameObject owner, Action callback)
        {
            if (!_noArgEvents.TryGetValue(eventName, out var list))
            {
                list = new List<EventBinding>();
                _noArgEvents[eventName] = list;
            }

            var binding = new EventBinding { owner = owner, Callback = callback };
            list.Add(binding);
            RegisterAutoUnsubscribe(eventName, owner, callback);
        }

        public static void AddEvent<T>(string eventName, GameObject owner, Action<T> callback)
        {
            if (!_oneArgEvents.TryGetValue(eventName, out var list))
            {
                list = new List<EventBinding>();
                _oneArgEvents[eventName] = list;
            }

            var binding = new EventBinding { owner = owner, Callback = callback };
            list.Add(binding);
            RegisterAutoUnsubscribe(eventName, owner, callback);
        }

        public static void RemoveEvent(string eventName, Delegate callback)
        {
            if (_noArgEvents.TryGetValue(eventName, out var noArgList))
                noArgList.RemoveAll(b => b.Callback == callback);

            if (_oneArgEvents.TryGetValue(eventName, out var oneArgList))
                oneArgList.RemoveAll(b => b.Callback == callback);
        }

        public static void Trigger(string eventName)
        {
            if (!_noArgEvents.TryGetValue(eventName, out var list)) return;

            var toRemove = new List<EventBinding>();

            foreach (var binding in list)
            {
                if (!IsValid(binding.owner))
                {
                    toRemove.Add(binding);
                    continue;
                }

                if (binding.Callback is Action action)
                    action.Invoke();
            }

            foreach (var item in toRemove)
                list.Remove(item);
        }

        public static void Trigger<T>(string eventName, T arg)
        {
            if (!_oneArgEvents.TryGetValue(eventName, out var list)) return;

            var toRemove = new List<EventBinding>();

            foreach (var binding in list)
            {
                if (!IsValid(binding.owner))
                {
                    toRemove.Add(binding);
                    continue;
                }

                if (binding.Callback is Action<T> callback)
                    callback.Invoke(arg);
            }

            foreach (var item in toRemove)
                list.Remove(item);
        }

        private static void RegisterAutoUnsubscribe(string eventName, GameObject owner, Delegate callback)
        {
            if (!owner) return;

            var proxy = owner.GetComponent<EventAutoUnsubscribeProxy>();
            if (!proxy)
                proxy = owner.AddComponent<EventAutoUnsubscribeProxy>();

            proxy.Register(() => RemoveEvent(eventName, callback));
        }

        private static bool IsValid(UnityEngine.Object owner)
        {
            if (!owner)
                return false;

            return owner switch
            {
                GameObject go => go.activeInHierarchy,
                Component component => component.gameObject.activeInHierarchy,
                _ => false
            };
        }
    }
}
