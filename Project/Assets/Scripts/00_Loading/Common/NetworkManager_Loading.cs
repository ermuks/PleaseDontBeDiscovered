using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class NetworkManager_Loading : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.SetPlayerCustomProperties(new ExitGames.Client.Photon.Hashtable()
        {
            { "isReady", false },
            { "isDead", false },
            { "isMurder", false },
            { "alreadyVoted", false },
            { "voteMembers", 0 }
        });
        EventManager.SendEvent("OpenScene", "Main");
    }
}
