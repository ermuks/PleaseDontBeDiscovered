using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PublicInventory : MonoBehaviourPun, IPunObservable
{
    public Inventory inventory;
    public int index;

    private void Awake()
    {
        index = photonView.ViewID;
        inventory = new Inventory(9);
        inventory.AddItem("0002", 9);
        EventManager.AddEvent("InventoryData :: SendData" + $"{index}", (p) =>
        {
            photonView.RPC("GetInventoryData", RpcTarget.All, (string)p[0], index);
        });
    }

    [PunRPC]
    private void GetInventoryData(string json, int index)
    {
        Debug.Log(json);
        Debug.Log(index);
        if (this.index == index)
        {
            InventoryInfo inventoryData = JsonUtility.FromJson<InventoryInfo>(json);
            for (int i = 0; i < inventoryData.cells.Length; i++)
            {
                if (inventoryData.cells[i].itemCode != "")
                {
                    inventory[i].data = ItemManager.GetItem(inventoryData.cells[i].itemCode);
                    inventory[i].itemCount = inventoryData.cells[i].itemCount;
                }
                else
                {
                    inventory[i].data = new ItemData("", "", "", -1, "");
                    inventory[i].itemCount = 0;
                }
            }
            EventManager.SendEvent("InventoryUI :: Refresh");
        }
    }

    public string InventoryToJson()
    {
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
            data.index = index;
        }
        return JsonUtility.ToJson(data);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(InventoryToJson());
        }
        else
        {
            InventoryInfo inventoryData = JsonUtility.FromJson<InventoryInfo>((string)stream.ReceiveNext());
            for (int i = 0; i < inventoryData.cells.Length; i++)
            {
                if (inventoryData.cells[i].itemCode != "")
                {
                    inventory[i].data = ItemManager.GetItem(inventoryData.cells[i].itemCode);
                    inventory[i].itemCount = inventoryData.cells[i].itemCount;
                }
                else
                {
                    inventory[i].data = new ItemData("", "", "", -1, "");
                    inventory[i].itemCount = 0;
                }
            }
            EventManager.SendEvent("InventoryUI :: Refresh");
        }
    }
}
