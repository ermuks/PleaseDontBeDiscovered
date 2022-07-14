using System;
using System.Collections.Generic;

public class ItemManager
{
    private static List<ItemData> data = new List<ItemData>();
    public static ItemData GetItem(string itemCode) => data.Find(e => e.itemCode == itemCode);
    public static void AddItem(ItemData item) => data.Add(item);

    public static void SetItemRandom()
    {
        string itemCode = "";
        int count = 0;
        float rnd = UnityEngine.Random.Range(0f, 100f);
        if (rnd < 25)
        {
            itemCode = "0000";
            count = 1;
        }
        else if (rnd < 40)
        {
            itemCode = "0000";
            count = 2;
        }
        else if (rnd < 45)
        {
            itemCode = "0001";
            count = 1;
        }
        else if (rnd < 46)
        {
            itemCode = "0001";
            count = 2;
        }
        else if (rnd < 75)
        {
            itemCode = "0002";
            count = 1;
        }
        else if (rnd < 82)
        {
            itemCode = "0003";
            count = 1;
        }
        else if (rnd < 83)
        {
            itemCode = "0003";
            count = 2;
        }
        else if (rnd < 84)
        {
            itemCode = "0004";
            count = 1;
        }
        else
        {
            itemCode = "0006";
            count = 1;
        }
        EventManager.SendEvent("Inventory :: AddItem", itemCode, count);
    }
}