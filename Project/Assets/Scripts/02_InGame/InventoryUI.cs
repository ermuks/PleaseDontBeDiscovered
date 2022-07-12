using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    private PublicInventory photonInventory;

    [SerializeField]
    private Transform cellsParent;
    private InventoryCells[] cells;

    [SerializeField]
    private Transform myCellsParent;
    private InventoryCells[] myCells;

    [SerializeField]
    private Image imgItemShadow;

    [SerializeField]
    private TMP_Text txtItemName;
    [SerializeField]
    private TMP_Text txtItemDescript;

    private void Awake()
    {
        cells = new InventoryCells[9];
        myCells = new InventoryCells[2];

        imgItemShadow.gameObject.SetActive(false);
        txtItemName.text = "";
        txtItemDescript.text = "";
        EventManager.AddEvent("InventoryUI :: Refresh", (p) => Refresh());
        EventManager.AddEvent("InventoryUI :: AddItem", (p) => AddItem((string)p[0]));
        EventManager.AddEvent("InventoryUI :: Remove", (p) => Remove((int)p[0]));
        EventManager.AddEvent("InventoryUI :: ShowItemInfo", (p) =>
        {
            ItemData data = (ItemData)p[0];
            txtItemName.text = data.itemName;
            txtItemDescript.text = data.itemDescript;
        });
        EventManager.AddEvent("InventoryUI :: ClearItemInfo", (p) =>
        {
            txtItemName.text = "";
            txtItemDescript.text = "";
        });
        EventManager.AddEvent("InventoryUI :: BeginDrag", (p) =>
        {
            imgItemShadow.gameObject.SetActive(true);
            imgItemShadow.sprite = (Sprite)p[0];
        });
        EventManager.AddEvent("InventoryUI :: FollowItem", (p) =>
        {
            Vector2 pos = (Vector2)p[0];
            imgItemShadow.transform.position = pos;
        });
        EventManager.AddEvent("InventoryUI :: EndDrag", (p) =>
        {
            imgItemShadow.gameObject.SetActive(false);
        });
        EventManager.AddEvent("InventoryUI :: SwapItem", (p) =>
        {
            int dragIndex = (int)p[0];
            int dropIndex = (int)p[1];
            inventory.SwapItem(dragIndex, dropIndex);
            Refresh();
        });
        EventManager.AddEvent("InventoryUI :: SendData", (p) =>
        {
            EventManager.SendEvent("InventoryData :: SendData" + $"{photonInventory.index}", photonInventory.InventoryToJson());
        });

        EventManager.AddData("InventoryUI >> TryAddItem", (p) => inventory.TryAddItem((string)p[0]));
        EventManager.AddData("InventoryUI >> TryAddItemMulty", (p) => inventory.TryAddItemMulty((string)p[0]));
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.CancelWork)))
        {
            EventManager.SendEvent("Player :: WorkEnd");
            EventManager.SendEvent("InGameUI :: CloseInventory");
            ResetEvent();
        }
    }

    public void Init(PublicInventory inven)
    {
        photonInventory = inven;
        Inventory myInventory = (Inventory)EventManager.GetData("Inventory");
        inventory = inven.inventory;
        for (int i = 0; i < cellsParent.childCount; i++)
        {
            if (cells[i] == null) cells[i] = cellsParent.GetChild(i).GetComponent<InventoryCells>();
            cells[i].Init(inventory[i], i, false);
        }
        for (int i = 0; i < myCellsParent.childCount; i++)
        {
            if (myCells[i] == null) myCells[i] = myCellsParent.GetChild(i).GetComponent<InventoryCells>();
            myCells[i].Init(myInventory[i], i, true);
        }
    }

    private void AddItem(string code)
    {
        inventory.AddItem(code, 1);
        Refresh();
    }

    private void Remove(int index)
    {
        inventory.Remove(index, 1);
        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < cellsParent.childCount; i++)
        {
            if (cells[i] == null) cells[i] = cellsParent.GetChild(i).GetComponent<InventoryCells>();
            cells[i].Refresh();
        }
        for (int i = 0; i < myCellsParent.childCount; i++)
        {
            if (myCells[i] == null) myCells[i] = myCellsParent.GetChild(i).GetComponent<InventoryCells>();
            myCells[i].Refresh();
        }
    }

    public void ResetEvent()
    {
        for (int i = 0; i < cellsParent.childCount; i++)
        {
            if (cells[i] == null) cells[i] = cellsParent.GetChild(i).GetComponent<InventoryCells>();
            cells[i].ResetEvent();
        }
        for (int i = 0; i < myCellsParent.childCount; i++)
        {
            if (myCells[i] == null) myCells[i] = myCellsParent.GetChild(i).GetComponent<InventoryCells>();
            myCells[i].ResetEvent();
        }
    }
}
