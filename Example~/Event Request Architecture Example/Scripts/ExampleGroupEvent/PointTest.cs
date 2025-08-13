using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleGroupEvent
{
    public class PointTest : MonoBehaviour
    {
        [SerializeField] private string message;

        private void Awake()
        {
            EventManager.Subscribe("PrintMessage", gameObject, PrintMessage);
        }

        private void PrintMessage()
        {
            print(message);
        }
    }
}
