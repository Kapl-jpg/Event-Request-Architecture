using ERA;
using UnityEngine;

public class CallTempData: MonoBehaviour
{
    public void PrintData()
    {
        RequestManager.TryGetValue("Points", out int points);
        print(points);
    }
}
