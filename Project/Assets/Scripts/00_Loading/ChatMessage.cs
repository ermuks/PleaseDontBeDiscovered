using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Realtime;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private Image imgContentBackground;
    [SerializeField] private Text txtNickname;
    [SerializeField] private Text txtContent;

    public void SetMessage(Player player, string message, bool richText, bool inGame = false)
    {
        string nickname = "";
        if (player != null)
        {
            if (player.IsMasterClient && !inGame)
            {
                imgContentBackground.color = new Color(.9f, .8f, .8f);
                txtNickname.color = new Color(.9f, .8f, .8f);
            }
            nickname = player.NickName;
        }

        if (txtNickname != null) txtNickname.text = nickname;
        txtContent.text = message;
        txtContent.supportRichText = richText;
    }

    public void SetMessage(string message, bool richText)
    {
        SetMessage(null, message, richText);
    }
}
