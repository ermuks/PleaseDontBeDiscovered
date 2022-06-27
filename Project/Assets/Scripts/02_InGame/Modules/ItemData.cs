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

    public ItemData(string itemCode, string itemName, string itemDescript, string eventKey)
    {
        this.itemCode = itemCode;
        this.itemName = itemName;
        this.itemDescript = itemDescript;
        itemImage = Resources.Load<Sprite>($"Sprites/02_InGame/Items/{itemCode}");
        this.eventKey = eventKey;
    }

    public void UseItem() => EventManager.SendEvent(eventKey);
}