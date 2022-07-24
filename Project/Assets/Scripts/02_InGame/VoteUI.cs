using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using TMPro;

public class VoteUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text txtVoteTime;
    [SerializeField] private Image imgVoteGauge;

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

            string txtColor = "ffffff";
            Color imgColor = Color.white;
            float remainTime = maxTime - timer;
            if (remainTime >= 5)
            {
                imgColor = Color.white;
                txtColor = "ffffff";
            }
            else if (timer >= maxTime / 2)
            {
                imgColor = new Color(224f, 144f, 163f, 255f) / 255f;
                txtColor = "e090a3";
            }
            else
            {
                imgColor = new Color(255f, 204f, 136f, 255f) / 255f;
                txtColor = "ffcc88";
            }
            txtVoteTime.text = Strings.GetString(StringKey.InGameVoteTimer, $"{maxTime - timer:0}", txtColor);
            imgVoteGauge.color = imgColor;
            imgVoteGauge.fillAmount = 1 - timer / maxTime;
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
