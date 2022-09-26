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
    [SerializeField] private Image imgPlayerColor;
    [SerializeField] private TMP_Text txtNickname;
    [SerializeField] private TMP_Text txtContent;

    public void SetMessage(Player player, string message, bool richText, bool inGame = false)
    {
        string nickname = "";
        bool deadPlayer = (bool)player.CustomProperties["isDead"];
        bool isMurderer = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurderer"];
        if (player != null)
        {
            if (player.IsMasterClient && !inGame)
            {
                imgContentBackground.color = new Color(.9f, .8f, .8f);
                txtNickname.color = new Color(.4f, .3f, .3f);
            }
            if (deadPlayer)
            {
                imgContentBackground.color = new Color(.4f, .3f, .3f, .4f);
                txtNickname.color = new Color(.9f, .8f, .8f, .75f);
                txtContent.color = Color.white;
            }
            if (isMurderer)
            {
                if ((bool)player.CustomProperties["isMurderer"])
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
        imgPlayerColor.color = PlayerData.GetColor(player);
    }

    public void SetMessage(string message, bool richText)
    {
        txtContent.text = message;
        txtContent.richText = richText;
    }
}
