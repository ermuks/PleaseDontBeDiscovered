using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private Text txtDebug;

    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            txtDebug.text = 
                "[ Room ]\n" +
                PhotonNetwork.CurrentRoom.CustomProperties.ToStringFull() + 
                "\n\n" +
                "[ Player ]\n" +
                PhotonNetwork.LocalPlayer.CustomProperties.ToStringFull();
        }
        else
        {
            txtDebug.text =
                "[ Player ]\n" +
                PhotonNetwork.LocalPlayer.CustomProperties.ToStringFull();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            foreach (var item in PhotonNetwork.LocalPlayer.CustomProperties)
            {
                Debug.Log($"{item.Key} :: {item.Value}");
            }
            EventManager.SendEvent("InGameUI :: CreateMessage", "����׿� ��ư (F3)");
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            foreach (var p in PhotonNetwork.CurrentRoom.Players)
            {
                foreach (var pr in p.Value.CustomProperties)
                {
                    Debug.Log($"{p.Value.NickName} :: {pr.Key} :: {pr.Value}");
                }
            }
            EventManager.SendEvent("InGameUI :: CreateMessage", "����׿� ��ư (F4)");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            foreach (var p in PhotonNetwork.CurrentRoom.CustomProperties)
            {
                Debug.Log($"{p.Key} :: {p.Value}");
            }
            EventManager.SendEvent("InGameUI :: CreateMessage", "����׿� ��ư (F5)");
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log(MissionManager.GetMissionToString());
            EventManager.SendEvent("InGameUI :: CreateMessage", "����׿� ��ư (F6)");
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            var properties = PhotonNetwork.CurrentRoom.CustomProperties;
            properties["Vote"] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            EventManager.SendEvent("InGameUI :: CreateMessage", "����׿� ��ư (F7)");
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            var eventList = EventManager.GetEventList();
            string result = "";
            foreach (var item in eventList.Keys)
            {
                result += $"{item}\n";
            }
            Debug.Log(result);
            EventManager.SendEvent("InGameUI :: CreateMessage", "����׿� ��ư (F8)");
        }
    }
}