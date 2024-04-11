
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] UnityEvent Event;

    public void TriggerEvent()
    {
        Event.Invoke();
    }
}
