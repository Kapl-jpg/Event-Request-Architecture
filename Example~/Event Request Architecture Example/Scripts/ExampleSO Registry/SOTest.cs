using UnityEngine;

public class SOTest: MonoBehaviour
{
    private SomeData _obj;

    private void Start()
    {
        _obj = SOManager.Get<SomeData>("SomeData");
        
        if (_obj)
        {
            print(_obj.points);
        }
    }
}
