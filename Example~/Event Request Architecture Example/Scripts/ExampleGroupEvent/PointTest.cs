using UnityEngine;

namespace Example.Event_Driven_Architecture_Example.Scripts.ExampleGroupEvent
{
    public class PointTest : Subscriber
    {
        [SerializeField] private string message;

        [Event("PrintMessage")]
        private void PrintMessage()
        {
            print(message);
        }
    }
}
