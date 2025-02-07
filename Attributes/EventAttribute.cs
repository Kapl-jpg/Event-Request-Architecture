using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class EventAttribute : Attribute
{
    public bool ObjectID { get; }
    public string ObjectName { get; }
    public string EventName { get; }

    public EventAttribute(string eventName) => EventName = eventName;

    public EventAttribute(string objectName, string eventName)
    {
        ObjectName = objectName;
        EventName = eventName;
    }

    public EventAttribute(bool getID, string eventName)
    {
        ObjectID = getID;
        EventName = eventName;
    }

    public string GetEventNameByID(string id)
    {
        return !ObjectID ? EventName : $"{id}.{EventName}";
    }
        
    public string GetEventName()
    {
        return string.IsNullOrEmpty(ObjectName) ? EventName : $"{ObjectName}.{EventName}";
    }
}