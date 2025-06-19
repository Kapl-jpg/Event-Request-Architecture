using System.Collections;
using UnityEngine;
public class TempData : Subscriber
{
    [TempRequest("Points")]
    [SerializeField] private ObservableField<int> points;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        print("The temporary object has been deleted");
        Destroy(gameObject);
    }
}
