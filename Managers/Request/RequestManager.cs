using System;
using System.Collections.Generic;

public static class RequestManager
{
    private static readonly Dictionary<string, object> Values = new();
    private static readonly Dictionary<string, Action<object>> UpdateHandlers = new();

    public static void SetValue<T>(string requestName, T value)
    {
        Values[requestName] = value;

        if (UpdateHandlers.TryGetValue(requestName, out var handler))
        {
            handler?.Invoke(value);
        }
    }

    public static T GetValue<T>(string requestName)
    {
        if (Values.TryGetValue(requestName, out var value))
        {
            return (T)value;
        }

        return default;
    }
}