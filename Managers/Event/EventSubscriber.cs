using System;
using System.Collections.Generic;
using UnityEngine;

namespace Event_Request_Architecture.Managers.Event
{
    public class EventSubscriber: MonoBehaviour
    {
        private EventData _eventData = new();

        public void AddEvent(EventName eventName, Delegate action, bool isPrivate = false)
        {
            _eventData.eventObject = gameObject;
            _eventData.isPrivate = isPrivate;
            _eventData.Delegates = new Dictionary<EventName, List<Delegate>>();

            if (_eventData.Delegates.ContainsKey(eventName))
            {
                _eventData.Delegates[eventName].Add(action);
            }
            else
            {
                List<Delegate> delegates = new List<Delegate> { action };
                _eventData.Delegates.Add(eventName, delegates);
            }
        }
    }
}