using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using TMPro;

public class VoteUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text txtVoteTime;

    bool isVoting = false;
    float timer = .0f;
    float maxTime = .0f;

    private void Awake()
    {
        maxTime = (float)PhotonNetwork.CurrentRoom.CustomProperties["voteTime"];
        EventManager.AddEvent("InGameUI :: Vote :: InitTimer", (p) =>
        {
            isVoting = true;
            timer = 0;
        });
    }

    private void Update()
    {
        if (isVoting) 
        {
            timer += Time.deltaTime;
            if (timer >= maxTime)
            {
                isVoting = false;
                if (PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                    roomProperties["Vote"] = false;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                }
            }
            txtVoteTime.text = Strings.GetString(StringKey.InGameVoteTimer, $"{maxTime - timer:0}");
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (!(bool)propertiesThatChanged["Vote"])
        {
            if (isVoting) isVoting = false;
            if (timer < maxTime) timer = 100000000;
        }
    }
}
