using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleGroupEvent
{
    public class CallEvent : MonoBehaviour
    {
        public void Call()
        {
            EventManager.Trigger("PrintMessage");
        }
    }
}
