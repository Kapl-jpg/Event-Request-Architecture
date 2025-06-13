using ERA;
using UnityEngine;
using Random = UnityEngine.Random;

public class PointsData : MonoBehaviour
{
    private readonly ObservableField<int> _points = new();

    private void Awake()
    {
        EventManager.AddEvent<int>("ChangePoints",gameObject, ChangePoints);
        EventManager.AddEvent("ChangePoints",gameObject, ChangePoints);
        _points.Init("Points", gameObject);
    }

    private void ChangePoints()
    {
        _points.Value = Random.Range(0, 1001);
        print($"New points.Value: {_points.Value}");
    }
    
    private void ChangePoints(int value)
    {
        print(value);
    }
}
