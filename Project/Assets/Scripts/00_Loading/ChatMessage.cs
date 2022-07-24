using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private Image imgContentBackground;
    [SerializeField] private TMP_Text txtNickname;
    [SerializeField] private TMP_Text txtContent;

    public void SetMessage(Player player, string message, bool richText, bool inGame = false)
    {
        string nickname = "";
        bool deadPlayer = (bool)player.CustomProperties["isDead"];
        bool isMurderer = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurder"];
        if (player != null)
        {
            if (player.IsMasterClient && !inGame)
            {
                imgContentBackground.color = new Color(.9f, .8f, .8f);
                txtNickname.color = new Color(.9f, .8f, .8f);
            }
            if (deadPlayer)
            {
                imgContentBackground.color = new Color(1f, .8f, .8f, .4f);
                txtNickname.color = new Color(.9f, .8f, .8f, .75f);
            }
            if (isMurderer)
            {
                if ((bool)player.CustomProperties["isMurder"])
                {
                    if ((bool)player.CustomProperties["isDead"])
                    {
                        txtNickname.color = new Color(1f, .2f, .25f);
                    }
                    else
                    {
                        txtNickname.color = new Color(1f, .2f, .25f, .75f);
                    }
                }
            }
            nickname = player.NickName;
        }

        if (txtNickname != null) txtNickname.text = nickname;
        txtContent.text = message;
        txtContent.richText = richText;
    }

    public void SetMessage(string message, bool richText)
    {
        SetMessage(null, message, richText);
    }
}
