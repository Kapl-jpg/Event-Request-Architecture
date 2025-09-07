using UnityEngine;
using Random = UnityEngine.Random;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleRequest
{
    public class PointsData : MonoBehaviour
    {
        private void Awake()
        {
            EventManager.Subscribe("ChangePoints", gameObject, ChangePoints);
            EventManager.Subscribe<int>("ChangePoints", gameObject, ChangePoints);
        }

        private void ChangePoints()
        {
            var random = Random.Range(0, 1001);
            RequestManager.SetValue("Points", random);
            print($"New points.Value: {random}");
        }

        private void ChangePoints(int value)
        {
            print(value);
        }
    }
}
