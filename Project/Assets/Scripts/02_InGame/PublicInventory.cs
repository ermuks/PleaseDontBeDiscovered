using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PublicInventory : MonoBehaviourPun, IPunObservable
{
    public Inventory inventory;

    private string prevData;
    private string inventoryJson;

    private void Awake()
    {
        inventory = new Inventory(9);
        inventory.AddItem("0002", 9);
    }

    public void SetOwner(Player player)
    {
        photonView.TransferOwnership(player);
    }

    private void FixedUpdate()
    {
        if (inventoryJson != "")
        {
            if (prevData != inventoryJson)
            {
                prevData = inventoryJson;
                InventoryInfo inventoryData;
                inventoryData = JsonUtility.FromJson<InventoryInfo>(inventoryJson);
                for (int i = 0; i < inventoryData.cells.Length; i++)
                {
                    inventory[i].data = ItemManager.GetItem(inventoryData.cells[i].itemCode);
                    inventory[i].itemCount = inventoryData.cells[i].itemCount;
                }
                EventManager.SendEvent("InventoryUI :: Refresh");
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Debug.Log("B");
            InventoryInfo data = new InventoryInfo();
            data.cells = new CellInfo[inventory.Count];
            for (int i = 0; i < inventory.Count; i++)
            {
                data.cells[i] = new CellInfo();
                if (inventory[i].itemCount > 0)
                {
                    data.cells[i].itemCode = inventory[i].data.itemCode;
                    data.cells[i].itemCount = inventory[i].itemCount;
                }
                else
                {
                    data.cells[i].itemCode = "";
                    data.cells[i].itemCount = 0;
                }
            }
            stream.SendNext(JsonUtility.ToJson(data));
        }
        else
        {
            inventoryJson = (string)stream.ReceiveNext();
        }
    }
}
