using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class TempRequestAttribute : Attribute
{
    public bool ObjectID { get; }
    public string ObjectName { get; }
    public string RequestName { get; }

    public TempRequestAttribute(string eventName) => RequestName = eventName;

    public TempRequestAttribute(string objectName, string eventName)
    {
        ObjectName = objectName;
        RequestName = eventName;
    }

    public TempRequestAttribute(bool getID, string eventName)
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