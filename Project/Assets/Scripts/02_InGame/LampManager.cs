using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class LampManager : MonoBehaviourPun
{
    private static LampManager instance;
    public static LampManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<LampManager>();
            return instance;
        }
    }

    private List<LampManagement> lampList = new List<LampManagement>();

    private void Awake()
    {
        EventManager.AddEvent("Lamp :: Murderer", (p) => SendLampMurderer((int)p[0]));
        EventManager.AddEvent("Lamp :: On", (p) => SendLampOn((int)p[0]));
        EventManager.AddEvent("Lamp :: Off", (p) => SendLampOff((int)p[0]));
    }

    public void AddLamp(LampManagement lamp)
    {
        lampList.Add(lamp);
    }

    private void SendLampMurderer(int viewID)
    {
        photonView.RPC("LampMurderer", RpcTarget.All, viewID);
    }

    private void SendLampOn(int viewID)
    {
        photonView.RPC("LampOn", RpcTarget.All, viewID);
    }

    private void SendLampOff(int viewID)
    {
        photonView.RPC("LampOn", RpcTarget.All, viewID);
    }

    [PunRPC]
    private void LampMurderer(int viewID)
    {
        for (int i = 0; i < lampList.Count; i++)
        {
            if (lampList[i].GetComponent<PhotonView>().ViewID == viewID)
            {
                lampList[i].ToggleLamp();
            }
        }
    }

    [PunRPC]
    private void LampOn(int viewID)
    {
        for (int i = 0; i < lampList.Count; i++)
        {
            if (lampList[i].GetComponent<PhotonView>().ViewID == viewID)
            {
                lampList[i].TurnOnLamp();
            }
        }
    }

    [PunRPC]
    private void LampOff(int viewID)
    {
        for (int i = 0; i < lampList.Count; i++)
        {
            if (lampList[i].GetComponent<PhotonView>().ViewID == viewID)
            {
                lampList[i].TurnOffLamp();
            }
        }
    }
}
