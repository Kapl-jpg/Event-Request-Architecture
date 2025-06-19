using UnityEngine;

namespace Example.Event_Driven_Architecture_Example.Scripts.ExampleGroupEvent
{
    public class CallEvent : MonoBehaviour
    {
        public void Call()
        {
            EventManager.Publish("PrintMessage");
        }
    }
}
