using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class JobMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtJobMessage;

    private void Awake()
    {
        bool murder = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMurderer"];
        txtJobMessage.text = murder ? Strings.GetString(StringKey.InGameMurderPlayer) : Strings.GetString(StringKey.InGameNoMurderPlayer);
        txtJobMessage.color = murder ? new Color(.733333f, .2f, .2f) : new Color(.2f, .733333f, .733333f);
    }

    public void End()
    {
        gameObject.SetActive(false);
    }
}
