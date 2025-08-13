using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleSO_Registry
{
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
}
