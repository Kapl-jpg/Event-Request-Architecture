using System;

public interface IObservableField
{
    object GetValue();
    void SetValue(object value);
    event Action<object> OnValueChanged;
}