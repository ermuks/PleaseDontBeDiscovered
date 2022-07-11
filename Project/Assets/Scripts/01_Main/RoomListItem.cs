using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomListItem : MonoBehaviourPun
{
    [SerializeField] private TMP_Text txtNumber;
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtPlayers;

    public void Refresh(int index, RoomInfo info)
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log(PhotonNetwork.LocalPlayer.UserId);
            Debug.Log(info.CustomProperties.Count);
            foreach (var v in info.CustomProperties)
            {
                Debug.Log($"{v.Key} :: {v.Value}");
            }
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(info.Name);
        });
        txtNumber.text = index.ToString();
        txtTitle.text = info.Name;
        txtPlayers.text = $"{info.PlayerCount} / {info.MaxPlayers}";
    }
}
