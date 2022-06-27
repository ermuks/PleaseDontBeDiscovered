using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetCheckTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeepWater"))
        {
            EventManager.SendEvent("Player :: EnterWet");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DeepWater"))
        {
            EventManager.SendEvent("Player :: ExitWet");
        }
    }
}
