using System;
using System.Collections.Generic;
using UnityEngine;

public static class TempManager
{
    private static readonly Dictionary<string, object> Values = new();
    private static readonly Dictionary<string, Action<object>> UpdateHandlers = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Application.quitting += OnApplicationQuit;
    }

    private static void OnApplicationQuit()
    {
        Values.Clear();
        UpdateHandlers.Clear();
    }

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

    public static void RemoveValue(string requestName)
    {
        if (Values.Remove(requestName))
        {
            Debug.Log($"TempManager: The value with the key '{requestName}' has been deleted.");
        }
        else
        {
            Debug.LogWarning($"TempManager: No value with the key '{requestName}' was found.");
        }
        
        UpdateHandlers.Remove(requestName);
    }
}