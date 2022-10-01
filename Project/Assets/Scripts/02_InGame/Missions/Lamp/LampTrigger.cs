using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LampTrigger : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<NormalLamp>() != null)
        {
            MissionUILamp.Instance.PutInLamp();
        }
    }
}
