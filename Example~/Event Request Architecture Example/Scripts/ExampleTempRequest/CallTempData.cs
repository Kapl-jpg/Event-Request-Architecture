using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleTempRequest
{
    public class CallTempData: MonoBehaviour
    {
        public void PrintData()
        {
            var points = RequestManager.GetValue<int>("Points");
            print(points);
        }
    }
}
