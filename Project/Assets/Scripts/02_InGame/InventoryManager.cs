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
        inventory = new Inventory(2);

        EventManager.AddEvent("Inventory :: AddItem", (p) => AddItem((string)p[0], (int)p[1]));
        EventManager.AddEvent("Inventory :: RemoveItem", (p) => RemoveItem((int)p[0], (int)p[1]));
        EventManager.AddEvent("Inventory :: UseItem", (p) => UseItem((int)p[0]));
        EventManager.AddEvent("Inventory :: Change", (p) => ChangeItem((string)p[0], (string)p[1]));
        EventManager.AddEvent("Inventory :: Remove", (p) => RemoveItem((string)p[0], 1));

        EventManager.AddEvent("Inventory :: HandWarmer", (p) => cells[(int)p[0]].UseHandWarmer(10f));

        EventManager.AddData("Inventory >> TryAddItem", (p) => inventory.TryAddItem((string)p[0]));
        EventManager.AddData("Inventory >> TryChange", (p) => inventory.TryChange((string)p[0], (string)p[1]));
        EventManager.AddData("Inventory >> HasItem", (p) => inventory.HasItem((string)p[0]));
        EventManager.AddData("Inventory >> FindIndex", (p) => inventory.FindIndex((string)p[0]));

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
                UseItem(0);
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
                UseItem(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem("0000", 1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            AddItem("0001", 1);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            AddItem("0002", 1);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            AddItem("0003", 1);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            AddItem("0004", 1);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            AddItem("0005", 1);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            AddItem("0006", 1);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            AddItem("0007", 1);
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            RemoveItem(0, 100);
            RemoveItem(1, 100);
        }
    }

    private void AddItem(string code, int count)
    {
        inventory.AddItem(code, count);
        Refresh();
    }

    private void RemoveItem(string code, int count)
    {
        inventory.Remove(code, count);
        Refresh();
    }

    private void RemoveItem(int index, int count)
    {
        inventory.Remove(index, count);
        Refresh();
    }

    private void UseItem(int index)
    {
        inventory.UseItem(index);
        Refresh();
    }

    private void ChangeItem(string origin, string code)
    {
        inventory.ChangeItem(origin, code);
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
