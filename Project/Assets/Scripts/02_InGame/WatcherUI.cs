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
        txtNickname.text = player.NickName;
    }
}
