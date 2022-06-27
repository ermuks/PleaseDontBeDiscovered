using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField]
    private ItemCellManager[] cells;

    private void Awake()
    {
        EventManager.AddData("Inventory >> ExistBlankCell", (p) =>
        {
            string code = (string)p[0];
            int count = (int)p[1];
            return inventory.ExistBlankCell(code, count);
        });
        EventManager.AddData("Inventory >> ExistItem", (p) =>
        {
            string code = (string)p[0];
            return inventory.ExistItem(code);
        });
        inventory = new Inventory(2);
        EventManager.AddEvent("Inventory :: AddItem", (p) =>
        {
            AddItem((string)p[0], (int)p[1]);
        });
        EventManager.AddEvent("Inventory :: RemoveItem", (p) =>
        {
            RemoveItem((int)p[0], (int)p[1]);
        });
        EventManager.AddEvent("Inventory :: UseItem", (p) =>
        {
            UseItem((int)p[0], (int)p[1]);
        });
        EventManager.AddEvent("Inventory :: Change", (p) =>
        {
            ChangeItem((string)p[0], (string)p[1], (bool)p[2]);
        });
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Init(i);
            cells[i].SetItem(inventory[i]);
            cells[i].Refresh();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.UseItem1)))
        {
            if ((bool)EventManager.GetData("Player :: Working"))
            {
                EventManager.SendEvent("InGameUI :: CreateMessage", Strings.GetString(StringKey.InGameMessageWorkingTryOtherAction));
            }
            else
            {
                UseItem(0, 1);
            }
        }
        if (Input.GetKeyDown(Settings.instance.GetKey(KeySettings.UseItem2)))
        {
            if ((bool)EventManager.GetData("Player :: Working"))
            {
                EventManager.SendEvent("InGameUI :: CreateMessage", Strings.GetString(StringKey.InGameMessageWorkingTryOtherAction));
            }
            else
            {
                UseItem(1, 1);
            }
        }
    }

    private void AddItem(string code, int count)
    {
        inventory.AddItem(code, count);
        Refresh();
    }

    private void RemoveItem(int index, int count)
    {
        inventory.Remove(index, count);
        Refresh();
    }

    private void UseItem(int index, int count)
    {
        inventory.UseItem(index, count);
        Refresh();
    }

    private void ChangeItem(string origin, string code, bool useable)
    {
        inventory.ChangeItem(origin, code, useable);
        Refresh();
    }

    private void Refresh()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            cells[i].Refresh();
        }
    }
}
