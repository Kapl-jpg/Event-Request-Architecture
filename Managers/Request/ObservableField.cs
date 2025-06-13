using System;
using UnityEngine;

namespace ERA
{
    [Serializable]
    public class ObservableField<T>
    {
        [SerializeField] private T value;

        private string _key;
        private GameObject _owner;
        private bool _initialized;
        private bool _isTemp;

        public Action<T> OnTypedValueChanged;

        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value)) return;

                this.value = value;
                OnTypedValueChanged?.Invoke(this.value);

                if (_initialized)
                {
                    if (_isTemp)
                        RequestManager.TempSetValue(_key, this.value);
                    else
                        RequestManager.SetValue(_key, this.value);
                }
            }
        }

        public void Init(string key, GameObject owner)
        {
            if (_initialized) return;

            _initialized = true;
            _key = key;
            _owner = owner;
            _isTemp = false;

            RequestManager.AddRequest(_key, _owner, this);
        }

        public void InitTemp(string key)
        {
            if (_initialized) return;

            _initialized = true;
            _key = key;
            _isTemp = true;

            if (RequestManager.TryGetValue(_key, out T existing))
                value = existing;

            RequestManager.TempSetValue(_key, value);
        }
    }
}