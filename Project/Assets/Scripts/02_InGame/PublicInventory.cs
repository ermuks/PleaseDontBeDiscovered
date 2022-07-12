using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PublicInventory : MonoBehaviourPun, IPunObservable
{
    public Inventory inventory;

    private void Awake()
    {
        inventory = new Inventory(9);
        inventory.AddItem("0002", 9);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
