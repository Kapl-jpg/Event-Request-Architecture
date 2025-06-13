using System.Collections;
using ERA;
using UnityEngine;

public class TempData : MonoBehaviour
{
    [SerializeField] private ObservableField<int> points;

    private void Awake()
    {
        points.InitTemp("Points");
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        print("The temporary object has been deleted");
        Destroy(gameObject);
    }
}
