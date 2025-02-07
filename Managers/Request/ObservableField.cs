using System;
using UnityEngine;

[Serializable]
public class ObservableField<T> : IObservableField
{
    [SerializeField] private T value;

    public Action<T> OnTypedValueChanged;
    public event Action<object> OnValueChanged;

    public T Value
    {
        get => value;
        set
        {
            if (Equals(this.value, value)) return;
            
            this.value = value;
            OnTypedValueChanged?.Invoke(this.value);
            OnValueChanged?.Invoke(this.value);
        }
    }

    public ObservableField(T initialValue = default)
    {
        value = initialValue;
    }

    object IObservableField.GetValue() => value;

    void IObservableField.SetValue(object value)
    {
        if (value is T typedValue)
        {
            Value = typedValue;
        }
    }
}