using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerData : MonoBehaviourPun, IPunObservable
{

    [PunRPC]
    private void Hit(Player player)
    {
        GetComponent<Animator>().SetTrigger("Hit");
        if ((bool)player.CustomProperties["isMurder"])
        {
            var p = PhotonNetwork.LocalPlayer.CustomProperties;
            p["isDead"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(p);
            EventManager.SendEvent("Player :: Die", player);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
