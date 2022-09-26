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
    [SerializeField] private Button btnSkip;

    private void Awake()
    {
        btnSkip.onClick.AddListener(() =>
        {
            bool isAlreadyVoted = (bool)PhotonNetwork.LocalPlayer.CustomProperties["alreadyVoted"];
            bool isDead = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"];
            if (!isAlreadyVoted && !isDead)
            {
                EventManager.SendEvent("InGameUI :: CompleteSkip", PhotonNetwork.LocalPlayer);
                EventManager.SendEvent("InGameUI :: CloseVoteButtons");
            }
            EventManager.SendEvent("InGameData :: AlreadyVoted", true);
        });

        EventManager.AddEvent("VoteUI :: RefreshTimer", (p) =>
        {
            float maxTime = (float)p[0];
            float timer = (float)p[1];

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
        });
    }

    public void EndVote()
    {
        var properties = PhotonNetwork.LocalPlayer.CustomProperties;
        properties["alreadyVoted"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        CloseUI();
    }

    private void CloseUI()
    {
        EventManager.SendEvent("InGameUI :: CloseVoteUI");
    }
}
