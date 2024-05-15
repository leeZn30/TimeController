using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperateTrigger : MonoBehaviour
{
    [SerializeField] Trigger TriggerObj;

    public void SimulateTrigger()
    {
        TriggerObj.TriggerEvent();
    }
}
