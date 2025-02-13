using System;
using System.Linq;
using System.Reflection;
using UnityEngine;


public abstract class Subscriber : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        SubscribeToEvents();
        SubscribeToRequests();
    }

    protected virtual void OnDisable()
    {
        UnsubscribeFromEvents();
        UnsubscribeFromRequests();
    }

    protected virtual void OnValidate()
    {
        ChangeRequest();
    }

    private void SubscribeToEvents()
    {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            foreach (var attribute in method.GetCustomAttributes(typeof(EventAttribute), false))
            {
                if (attribute is not EventAttribute eventAttribute) continue;

                string eventName = GetEventName(eventAttribute);
                var parameters = method.GetParameters();

                if (parameters.Length == 0)
                {
                    // Подписка для методов без параметров.
                    var action = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
                    EventManager.Subscribe(eventName, action);
                }
                else if (parameters.Length == 1)
                {
                    // Подписка для методов с одним параметром: извлекаем тип параметра.
                    Type paramType = parameters[0].ParameterType;
                    Type delegateType = typeof(Action<>).MakeGenericType(paramType);
                    var action = Delegate.CreateDelegate(delegateType, this, method);

                    // Находим обобщённый метод Subscribe<T>(string, Action<T>)
                    MethodInfo subscribeMethod = typeof(EventManager)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(m => m.Name == "Subscribe" &&
                                             m.IsGenericMethodDefinition &&
                                             m.GetParameters().Length == 2 &&
                                             m.GetParameters()[0].ParameterType == typeof(string));
                    if (subscribeMethod != null)
                    {
                        var genericSubscribeMethod = subscribeMethod.MakeGenericMethod(paramType);
                        genericSubscribeMethod.Invoke(null, new object[] { eventName, action });
                    }
                    else
                    {
                        Debug.LogError($"Не найден метод Subscribe для типа {paramType}");
                    }
                }
                else
                {
                    Debug.LogError("Методы с более чем одним параметром не поддерживаются для подписки на события.");
                }
            }
        }
    }

    private void UnsubscribeFromEvents()
    {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            foreach (var attribute in method.GetCustomAttributes(typeof(EventAttribute), false))
            {
                if (attribute is not EventAttribute eventAttribute) continue;

                string eventName = GetEventName(eventAttribute);
                var parameters = method.GetParameters();

                if (parameters.Length == 0)
                {
                    var action = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
                    EventManager.Unsubscribe(eventName, action);
                }
                else if (parameters.Length == 1)
                {
                    Type paramType = parameters[0].ParameterType;
                    Type delegateType = typeof(Action<>).MakeGenericType(paramType);
                    var action = Delegate.CreateDelegate(delegateType, this, method);

                    MethodInfo unsubscribeMethod = typeof(EventManager)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(m => m.Name == "Unsubscribe" &&
                                             m.IsGenericMethodDefinition &&
                                             m.GetParameters().Length == 2 &&
                                             m.GetParameters()[0].ParameterType == typeof(string));
                    if (unsubscribeMethod != null)
                    {
                        var genericUnsubscribeMethod = unsubscribeMethod.MakeGenericMethod(paramType);
                        genericUnsubscribeMethod.Invoke(null, new object[] { eventName, action });
                    }
                    else
                    {
                        Debug.LogError($"Не найден метод Unsubscribe для типа {paramType}");
                    }
                }
                else
                {
                    Debug.LogError("Методы с более чем одним параметром не поддерживаются для отписки от событий.");
                }
            }
        }
    }

    // Пример работы с RequestAttribute оставляем без изменений.
    private void SubscribeToRequests()
    {
        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var attribute = field.GetCustomAttribute<RequestAttribute>();
            if (attribute == null) continue;

            string requestName = GetRequestName(attribute);
            var fieldValue = field.GetValue(this);

            if (fieldValue is not IObservableField observableField) continue;

            // Замечание: лямбда здесь создаётся заново, что может помешать корректной отписке.
            observableField.OnValueChanged += value => { RequestManager.SetValue(requestName, value); };
            RequestManager.SetValue(requestName, observableField.GetValue());
        }
    }
    
    private void UnsubscribeFromRequests()
    {
        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var attribute = field.GetCustomAttribute<RequestAttribute>();
            if (attribute == null) continue;

            string requestName = GetRequestName(attribute);
            var fieldValue = field.GetValue(this);

            if (fieldValue is not IObservableField observableField) continue;

            // Аналогичное замечание: отписка может не сработать, если делегат не сохранён.
            observableField.OnValueChanged -= value => { RequestManager.SetValue(requestName, value); };
        }
    }

    private void ChangeRequest()
    {
        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var attribute = field.GetCustomAttribute<RequestAttribute>();
            if (attribute == null) continue;

            string requestName = GetRequestName(attribute);
            var fieldValue = field.GetValue(this);

            if (fieldValue is not IObservableField observableField) continue;
            
            RequestManager.SetValue(requestName, observableField.GetValue());
        }
    }
    
    private string GetEventName(EventAttribute eventAttribute)
    {
        return eventAttribute.ObjectID
            ? eventAttribute.GetEventNameByID(gameObject.GetInstanceID().ToString())
            : eventAttribute.GetEventName();
    }

    private string GetRequestName(RequestAttribute requestAttribute)
    {
        return requestAttribute.ObjectID
            ? requestAttribute.GetEventNameByID(gameObject.GetInstanceID().ToString())
            : requestAttribute.GetEventName();
    }
}
