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
    }
}
