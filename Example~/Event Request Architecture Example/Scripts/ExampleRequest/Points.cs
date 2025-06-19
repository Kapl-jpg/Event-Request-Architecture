using UnityEngine;

namespace Example.Event_Driven_Architecture_Example.Scripts.ExampleRequest
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
            EventManager.Publish("ChangePoints",50);
        }
    }
}