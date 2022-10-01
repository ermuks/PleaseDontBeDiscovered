using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutsideLampTrigger : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<CrashedLamp>() != null)
        {
            MissionUILamp.Instance.TakeOutCrashLamp();
        }
    }
}
