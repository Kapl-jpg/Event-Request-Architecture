using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class RequestAttribute : Attribute
{
    public bool ObjectID { get; }
    public string ObjectName { get; }
    public string RequestName { get; }

    public RequestAttribute(string eventName) => RequestName = eventName;

    public RequestAttribute(string objectName, string eventName)
    {
        ObjectName = objectName;
        RequestName = eventName;
    }

    public RequestAttribute(bool getID, string eventName)
    {
        ObjectID = getID;
        RequestName = eventName;
    }

    public string GetEventNameByID(string id)
    {
        return !ObjectID ? RequestName : $"{id}.{RequestName}";
    }
        
    public string GetEventName()
    {
        return string.IsNullOrEmpty(ObjectName) ? RequestName : $"{ObjectName}.{RequestName}";
    }
}