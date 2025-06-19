using UnityEngine;

public class CallTempData: MonoBehaviour
{
    public void PrintData()
    {
        var points = TempManager.GetValue<int>("Points");
        print(points);
    }
}
