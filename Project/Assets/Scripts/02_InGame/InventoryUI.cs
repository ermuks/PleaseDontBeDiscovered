using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;

    [SerializeField]
    private Transform cellsParent;
    private InventoryCells[] cells;

    private void Awake()
    {
        cells = new InventoryCells[9];
        EventManager.AddEvent("InventoryUI :: OpenInventory", (p) =>
        {
            Debug.Log("¿­·È´Ù!");
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.CancelWork)))
        {
            EventManager.SendEvent("Player :: WorkEnd");
            EventManager.SendEvent("InGameUI :: CloseInventory");
        }
    }

    public void Init(Inventory inven)
    {
        inventory = inven;
        for (int i = 0; i < cellsParent.childCount; i++)
        {
            if (cells[i] == null) cells[i] = cellsParent.GetChild(i).GetComponent<InventoryCells>();
            cells[i].Init(inventory[i]);
            Debug.Log(inventory[i].data.itemCode);
        }
    }
}
