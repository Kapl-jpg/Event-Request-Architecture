using UnityEngine;

namespace Example.Event_Driven_Architecture_Example.Scripts.ExampleRequest
{
    public class PointsData : Subscriber
    {
        [Request("Points")] 
        private ObservableField<int> points = new();

        [Event("ChangePoints")]
        private void ChangePoints()
        {
            points.Value = Random.Range(0, 1001);
            print($"New points.Value: {points.Value}");
        }

        [Event("ChangePoints")]
        private void ChangePoints(int value)
        {
            print(value);
        }
    }
}
