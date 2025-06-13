using ERA;
using UnityEngine;

public class CallEvent : MonoBehaviour
{
    public void Call()
    {
        EventManager.Trigger("PrintMessage");
    }
}
