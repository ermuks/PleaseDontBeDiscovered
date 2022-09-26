using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class WatcherUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtNickname;

    public void SetPlayer(Player player)
    {
        if ((bool)player.CustomProperties["isMurderer"])
        {
            txtNickname.color = new Color(1f, .06f, .08f);
        }
        else
        {
            txtNickname.color = Color.white;
        }
        txtNickname.text = player.NickName;
    }
}
