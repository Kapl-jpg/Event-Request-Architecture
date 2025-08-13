using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleRequest
{
    public class Points : MonoBehaviour
    {
        public void GetPoints()
        {
            var points = RequestManager.GetValue<int>("Points");
            print($"Get points value: {points}");
        }

        public void ChangePoints()
        {
            EventManager.Trigger("ChangePoints");
            EventManager.Trigger("ChangePoints",50);
        }
    }
}