using ERA;
using UnityEngine;

public class Points : MonoBehaviour
{
    public void GetPoints()
    {
        RequestManager.TryGetValue("Points", out int points);
        print($"Get points value: {points}");
    }

    public void ChangePoints()
    {
        EventManager.Trigger("ChangePoints");
        EventManager.Trigger("ChangePoints", 50);
    }
}