using System.Collections.Generic;
using UnityEngine;

namespace ERA
{
    public static class RequestManager
    {
        private interface IRequestBinding
        {
            GameObject Owner { get; }
        }

        private class RequestBinding<T> : IRequestBinding
        {
            public GameObject Owner { get; set; }
            public ObservableField<T> Field { get; set; }
        }

        private static readonly Dictionary<string, object> _values = new();
        private static readonly Dictionary<string, List<IRequestBinding>> _bindings = new();
        private static readonly HashSet<string> _persistentKeys = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Application.quitting += () =>
            {
                _values.Clear();
                _bindings.Clear();
                _persistentKeys.Clear();
            };

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (_, _) =>
            {
                var tempValues = new Dictionary<string, object>();
                foreach (var key in _persistentKeys)
                {
                    if (_values.TryGetValue(key, out var val))
                        tempValues[key] = val;
                }

                _values.Clear();
                _bindings.Clear();

                foreach (var pair in tempValues)
                    _values[pair.Key] = pair.Value;
            };
        }

        public static void SetValue<T>(string key, T value)
        {
            _values[key] = value;

            if (_bindings.TryGetValue(key, out var baseList))
            {
                var toRemove = new List<IRequestBinding>();

                foreach (var raw in baseList)
                {
                    if (raw is not RequestBinding<T> binding)
                        continue;

                    if (!IsAlive(binding.Owner))
                    {
                        toRemove.Add(binding);
                        continue;
                    }

                    binding.Field.Value = value;
                }

                foreach (var dead in toRemove)
                    baseList.Remove(dead);
            }
        }

        public static void TempSetValue<T>(string key, T value)
        {
            _persistentKeys.Add(key);
            SetValue(key, value);
        }

        public static void ClearValue(string key)
        {
            _persistentKeys.Remove(key);
            _values.Remove(key);
        }

        public static bool TryGetValue<T>(string key, out T value)
        {
            if (_values.TryGetValue(key, out var raw) && raw is T casted)
            {
                value = casted;
                return true;
            }

            value = default;
            return false;
        }


        public static void AddRequest<T>(string key, GameObject owner, ObservableField<T> field)
        {
            if (!_bindings.TryGetValue(key, out var baseList))
            {
                baseList = new List<IRequestBinding>();
                _bindings[key] = baseList;
            }

            baseList.Add(new RequestBinding<T> { Owner = owner, Field = field });

            // Автоматическая отписка
            if (owner is { } go)
            {
                var proxy = go.GetComponent<RequestAutoUnsubscribeProxy>();
                if (!proxy)
                    proxy = go.AddComponent<RequestAutoUnsubscribeProxy>();

                proxy.Register(() => Unsubscribe(key, field));
            }

            if (_values.TryGetValue(key, out var value) && value is T typed)
            {
                field.Value = typed;
            }
        }

        private static void Unsubscribe<T>(string key, ObservableField<T> field)
        {
            if (!_bindings.TryGetValue(key, out var baseList))
                return;

            baseList.RemoveAll(b => b is RequestBinding<T> binding && binding.Field == field);

            if (baseList.Count == 0)
                _bindings.Remove(key);
        }

        private static bool IsAlive(GameObject obj) => obj;
    }
}
