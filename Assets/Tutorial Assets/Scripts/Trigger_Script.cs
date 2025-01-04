using UnityEngine;
using UnityEngine.Events;

public class Trigger_Script : MonoBehaviour
{
    [SerializeField] UnityEvent triggerEvent;
    [SerializeField] UnityEvent exitEvent;

    void OnTriggerStay(Collider other)
    {
        triggerEvent.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        exitEvent.Invoke();
    }
}
