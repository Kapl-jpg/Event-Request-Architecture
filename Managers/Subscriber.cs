using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class Subscriber : MonoBehaviour
{
    private bool _tempIsRegistry;
    protected virtual void OnEnable()
    {
        SubscribeToEvents();
        SubscribeToRequests();
        SubscribeToTemporaryRequests();
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
            foreach (var attribute in method.GetCustomAttributes(typeof(EventAttribute), true))
            {
                if (attribute is not EventAttribute eventAttribute) continue;

                string eventName = GetEventName(eventAttribute);
                var parameters = method.GetParameters();

                if (parameters.Length == 0)
                {
                    var action = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
                    EventManager.Subscribe(eventName, action);
                }
                else if (parameters.Length == 1)
                {
                    Type paramType = parameters[0].ParameterType;
                    Type delegateType = typeof(Action<>).MakeGenericType(paramType);
                    var action = Delegate.CreateDelegate(delegateType, this, method);

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
                        Debug.LogError($"No Subscribe method found for type {paramType}");
                    }
                }
                else
                {
                    Debug.LogError("Methods with more than one parameter are not supported for subscribing from events.");
                }
            }
        }
    }

    private void UnsubscribeFromEvents()
    {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var method in methods)
        {
            foreach (var attribute in method.GetCustomAttributes(typeof(EventAttribute), true))
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
                        Debug.LogError($"No Unsubscribe method found for type {paramType}");
                    }
                }
                else
                {
                    Debug.LogError("Methods with more than one parameter are not supported for unsubscribing from events");
                }
            }
        }
    }

    private void SubscribeToRequests()
    {
        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var attribute = field.GetCustomAttribute<RequestAttribute>();
            if (attribute == null) continue;

            string requestName = GetRequestName(attribute);
            var fieldValue = field.GetValue(this);

            if (fieldValue is not IObservableField observableField) continue;

            observableField.OnValueChanged += value => { RequestManager.SetValue(requestName, value); };
            RequestManager.SetValue(requestName, observableField.GetValue());
        }
    }

    private void SubscribeToTemporaryRequests()
    {
        if (_tempIsRegistry)
            return;
        
        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var attribute = field.GetCustomAttribute<TempRequestAttribute>();
            if (attribute == null) continue;

            string requestName = GetTempRequestName(attribute);
            var fieldValue = field.GetValue(this);

            if (fieldValue is not IObservableField observableField) continue;

            // Замечание: лямбда здесь создаётся заново, что может помешать корректной отписке.
            observableField.OnValueChanged += value => { TempManager.SetValue(requestName, value); };
            TempManager.SetValue(requestName, observableField.GetValue());
        }

        _tempIsRegistry = true;
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
    
    private string GetTempRequestName(TempRequestAttribute requestAttribute)
    {
        return requestAttribute.ObjectID
            ? requestAttribute.GetEventNameByID(gameObject.GetInstanceID().ToString())
            : requestAttribute.GetEventName();
    }
}
