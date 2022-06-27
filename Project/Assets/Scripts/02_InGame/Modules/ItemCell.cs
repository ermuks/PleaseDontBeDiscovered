using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCell
{
    public ItemData data;
    public int itemCount;

    public ItemCell()
    {
        Initialize();
    }

    public void Initialize()
    {
        data = new ItemData("", "", "", -1, "");
        itemCount = 0;
    }

    public void SetCell(ItemCell cell)
    {
        data = cell.data;
        itemCount = cell.itemCount;
    }
}