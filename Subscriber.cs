using System;
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
        SubscribeToRequests();
    }
    
    private void SubscribeToEvents()
    {
        foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                    BindingFlags.NonPublic))
        {
            foreach (var attribute in method.GetCustomAttributes(typeof(EventAttribute), false))
            {
                if (attribute is not EventAttribute eventAttribute) continue;

                var eventName = GetEventName(eventAttribute);
                if (method.GetParameters().Length == 0)
                {
                    var action = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
                    EventManager.Access.Subscribe(eventName, action);
                }
                else
                {
                    var actionWithParam = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), this, method);
                    EventManager.Access.Subscribe(eventName, actionWithParam);
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

            var requestName = GetRequestName(attribute);
            var fieldValue = field.GetValue(this);

            if (fieldValue is not IObservableField observableField) continue;

            observableField.OnValueChanged += value => { RequestManager.SetValue(requestName, value); };

            RequestManager.SetValue(requestName, observableField.GetValue());
        }
    }

    private void UnsubscribeFromEvents()
    {
        foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                                    BindingFlags.NonPublic))
        {
            foreach (var attribute in method.GetCustomAttributes(typeof(EventAttribute), false))
            {
                if (attribute is not EventAttribute eventAttribute) continue;

                var eventName = GetEventName(eventAttribute);

                if (method.GetParameters().Length == 0)
                {
                    var action = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
                    EventManager.Access.Unsubscribe(eventName, action);
                }
                else
                {
                    var actionWithParam = (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), this, method);
                    EventManager.Access.Unsubscribe(eventName, actionWithParam);
                }
            }
        }
    }
    
    private void UnsubscribeFromRequests()
    {
        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var attribute = field.GetCustomAttribute<RequestAttribute>();
            if (attribute == null) continue;

            var requestName = GetRequestName(attribute);
            var fieldValue = field.GetValue(this);

            if (fieldValue is not IObservableField observableField) continue;

            observableField.OnValueChanged -= value => { RequestManager.SetValue(requestName, value); };
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