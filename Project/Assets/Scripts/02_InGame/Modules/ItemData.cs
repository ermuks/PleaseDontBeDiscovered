using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public string itemCode;
    public string itemName;
    public string itemDescript;
    public Sprite itemImage;
    public string eventKey;

    public int maxCount = -1;

    public ItemData(string itemCode, string itemName, string itemDescript, int maxCount, string eventKey)
    {
        this.itemCode = itemCode;
        this.itemName = itemName;
        this.itemDescript = itemDescript;
        itemImage = Resources.Load<Sprite>($"Sprites/02_InGame/Items/{itemCode}");
        this.eventKey = eventKey;
        this.maxCount = maxCount;
    }

    public void UseItem() => EventManager.SendEvent(eventKey);
}