using System.Collections;
using UnityEngine;

namespace Event_Request_Architecture.Example.Event_Request_Architecture_Example.Scripts.ExampleTempRequest
{
    public class TempData : MonoBehaviour
    {
        [SerializeField] private int points;

        private IEnumerator Start()
        {
            RequestManager.SetValue("Points", points);
            yield return new WaitForSeconds(1f);
            print("The temporary object has been deleted");
            Destroy(gameObject);
        }
    }
}
