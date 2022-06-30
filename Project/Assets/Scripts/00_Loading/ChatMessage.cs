using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Photon.Realtime;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private Image imgNicknameBackground;
    [SerializeField] private Image imgContentBackground;
    [SerializeField] private TMP_Text txtNickname;
    [SerializeField] private TMP_Text txtContent;

    public void SetMessage(Player player, string message, bool richText, bool inGame = false)
    {
        string nickname = "";
        if (player != null)
        {
            if (player.IsMasterClient && !inGame)
            {
                imgNicknameBackground.color = new Color(.6f, .4f, .4f);
                imgContentBackground.color = new Color(.9f, .8f, .8f);
                txtNickname.color = new Color(.9f, .8f, .8f);
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