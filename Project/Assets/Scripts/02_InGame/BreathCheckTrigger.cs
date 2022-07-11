using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathCheckTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeepWater"))
        {
            EventManager.SendEvent("Player :: CantBreath");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DeepWater"))
        {
            EventManager.SendEvent("Player :: CanBreath");
        }
    }
}
