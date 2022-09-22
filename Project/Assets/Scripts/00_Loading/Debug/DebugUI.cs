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
            EventManager.SendEvent("InGameUI :: CreateMessage", "디버그용 버튼 (F3)");
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
            EventManager.SendEvent("InGameUI :: CreateMessage", "디버그용 버튼 (F4)");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            foreach (var p in PhotonNetwork.CurrentRoom.CustomProperties)
            {
                Debug.Log($"{p.Key} :: {p.Value}");
            }
            EventManager.SendEvent("InGameUI :: CreateMessage", "디버그용 버튼 (F5)");
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log(MissionManager.GetMissionToString());
            EventManager.SendEvent("InGameUI :: CreateMessage", "디버그용 버튼 (F6)");
        }
    }
}
