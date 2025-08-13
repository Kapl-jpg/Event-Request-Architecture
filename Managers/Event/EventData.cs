using System;
using System.Collections.Generic;
using UnityEngine;

namespace Event_Request_Architecture.Managers.Event
{
    [Serializable]
    public struct EventData
    {
        public GameObject eventObject;
        public bool isPrivate;
        public Dictionary<EventName, List<Delegate>> Delegates;
    }
}