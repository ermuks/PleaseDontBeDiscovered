using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerData : MonoBehaviourPun, IPunObservable
{
    private void Awake()
    {
        EventManager.AddEvent("Data :: Die", (p) =>
        {
            Die(p[0], (bool)p[1], (bool)p[2]);
        });
        EventManager.AddEvent("Data :: Hit", (p) =>
        {
            Hit((Player)p[0]);
        });
        EventManager.AddEvent("Data :: VoteDie", (p) =>
        {
            VoteDie();
        });
    }


    private void VoteDie()
    {
        Die(DieMessage.Vote, false, false);
    }

    private void Hit(Player player)
    {
        GetComponent<Animator>().SetTrigger("Hit");
        if ((bool)player.CustomProperties["isMurder"])
        {
            Die(player, true, true);
        }
    }

    private void Die(object obj, bool openUI, bool setDieAnim)
    {
        var p = PhotonNetwork.LocalPlayer.CustomProperties;
        p["isDead"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(p);

        if (obj.GetType() != typeof(Player)) obj = (DieMessage)obj;
        if (openUI) EventManager.SendEvent("Player :: Die", obj);
        if (setDieAnim) EventManager.SendEvent("InGameUI :: Die", obj);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}