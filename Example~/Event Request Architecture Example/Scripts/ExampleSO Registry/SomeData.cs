using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleSO_Registry
{
    [CreateAssetMenu(menuName = "Data", fileName = "SomeData")]
    public class SomeData: ScriptableObject
    {
        public int points = 100;
    }
}
