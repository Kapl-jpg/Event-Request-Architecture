using ERA;
using UnityEngine;

public class PointTest : MonoBehaviour
{
    [SerializeField] private string message;

    private void Start()
    {
        EventManager.AddEvent("PrintMessage", gameObject, PrintMessage);
    }

    private void PrintMessage()
    {
        print(message);
    }
}

