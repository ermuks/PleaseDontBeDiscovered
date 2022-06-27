using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInWater : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeepWater"))
        {
            EventManager.SendEvent("InGameUI :: InWater");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DeepWater"))
        {
            EventManager.SendEvent("InGameUI :: OutWater");
        }
    }
}