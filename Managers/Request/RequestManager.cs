using System;
using System.Collections.Generic;

public static class RequestManager
{
    // Хранение значений по имени запроса
    private static readonly Dictionary<string, object> _values = new();

    /// <summary>
    /// Устанавливает или обновляет значение запроса.
    /// </summary>
    public static void SetValue<T>(string requestName, T value)
    {
        if (_values.ContainsKey(requestName))
        {
            _values[requestName] = value;  // обновить
        }
        else
        {
            _values.Add(requestName, value);  // добавить
        }
    }

    /// <summary>
    /// Возвращает текущее значение запроса или default(T), если его нет.
    /// </summary>
    public static T GetValue<T>(string requestName)
    {
        if (_values.TryGetValue(requestName, out var obj) && obj is T typed)
            return typed;
        return default;
    }
}