using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class NetworkManager_Loading : MonoBehaviourPunCallbacks
{
    int color;

    private void Awake()
    {
        LoadData();
        PhotonNetwork.ConnectUsingSettings();
    }

    private void LoadData()
    {
        if (PlayerPrefs.HasKey("color"))
        {
            color = PlayerPrefs.GetInt("color");
        }
        else
        {
            color = Random.Range(0, 18);
            PlayerPrefs.SetInt("color", color);
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.SetPlayerCustomProperties(new ExitGames.Client.Photon.Hashtable()
        {
            { "isReady", false },
            { "isDead", false },
            { "isMurder", false },
            { "alreadyVoted", false },
            { "voteMembers", 0 },
            { "color", color }
        });
        Debug.Log((int)PhotonNetwork.LocalPlayer.CustomProperties["color"]);
        EventManager.SendEvent("OpenScene", "Main");
    }
}
