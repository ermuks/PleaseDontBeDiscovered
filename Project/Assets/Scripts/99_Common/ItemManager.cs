using System;
using System.Collections.Generic;

public class ItemManager
{
    private static List<ItemData> data = new List<ItemData>();
    public static ItemData GetItem(string itemCode) => data.Find(e => e.itemCode == itemCode);
    public static void AddItem(ItemData item) => data.Add(item);

    public static void SetItemRandom(int min, int max)
    {
        EventManager.SendEvent("Inventory :: AddItem", data[UnityEngine.Random.Range(0, data.Count)].itemCode, UnityEngine.Random.Range(min, max + 1));
    }
}