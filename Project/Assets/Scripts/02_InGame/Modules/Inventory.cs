using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public int gold;
    private ItemCell[] cells;
    public ItemCell this[int index] {
        set
        {
            cells[index] = value;
        }
        get
        {
            return cells[index];
        }
    }
    public int Count => cells.Length;

    public Inventory(int count)
    {
        cells = new ItemCell[count];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = new ItemCell();
        }
    }

    public void AddItem(string code, int count)
    {
        int remainCount = count;
        int index = -1;

        while (remainCount > 0)
        {
            index = System.Array.FindIndex(cells, index + 1, e => e.data.itemCode == code);
            if (index != -1)
            {
                if (cells[index].itemCount + remainCount > cells[index].data.maxCount)
                {
                    remainCount -= cells[index].data.maxCount - cells[index].itemCount;
                    cells[index].itemCount = cells[index].data.maxCount;
                }
                else
                {
                    cells[index].itemCount += remainCount;
                    remainCount = 0;
                }
            }
            else
            {
                index = System.Array.FindIndex(cells, e => e.itemCount == 0);
                if (index != -1)
                {
                    ItemData data = ItemManager.GetItem(code);
                    cells[index].data = data;
                    if (remainCount > data.maxCount)
                    {
                        remainCount -= data.maxCount;
                        cells[index].itemCount = data.maxCount;
                    }
                    else
                    {
                        cells[index].itemCount = remainCount;
                        remainCount = 0;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void Remove(string code, int removeCount)
    {
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == code);
        Remove(index, removeCount);
    }

    public void Remove(int index, int removeCount)
    {
        if (index != -1)
        {
            int itemCount = cells[index].itemCount;
            if (itemCount <= removeCount)
            {
                cells[index].Initialize();
            }
            else
            {
                cells[index].itemCount -= removeCount;
            }
        }
    }

    public void SwapItem(int dragIndex, int dropIndex)
    {
        ItemCell cell = new ItemCell();

        cell.SetCell(cells[dragIndex]);
        cells[dragIndex].SetCell(cells[dropIndex]);
        cells[dropIndex].SetCell(cell);
    }

    public void UseItem(int index)
    {
        if (cells[index].itemCount >= 0)
        {
            cells[index].data.UseItem();
        }
    }

    public void ChangeItem(string origin, string code)
    {
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == origin);
        if (index != -1)
        {
            Remove(origin, 1);
            int blankIndex = System.Array.FindIndex(cells, e => e.itemCount == 0);
            if (blankIndex != -1)
            {
                AddItem(code, 1);
            }
            else
            {
                int existIndex = System.Array.FindIndex(cells, e => e.data.itemCode == code);
                if (existIndex != -1)
                {
                    AddItem(code, 1);
                }
                else
                {
                    AddItem(origin, 1);
                }
            }
        }
    }

    public bool TryChange(string origin, string code)
    {
        bool result = true;
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == code);
        if (index != -1)
        {
            if (cells[index].data.maxCount != -1)
            {
                if (cells[index].itemCount >= cells[index].data.maxCount)
                {
                    result = false;
                }
            }
        }
        else
        {
            index = System.Array.FindIndex(cells, e => e.itemCount == 0);
            if (index == -1)
            {
                if (ItemCount(origin) != 1)
                {
                    result = false;
                }
            }
        }
        return result;
    }

    public bool TryAddItemMulty(string code)
    {
        bool result = true;
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == code);
        if (index != -1)
        {
            if (cells[index].data.maxCount != -1)
            {
                if (cells[index].itemCount >= cells[index].data.maxCount)
                {
                    index = System.Array.FindIndex(cells, e => e.itemCount == 0);
                    if (index == -1)
                    {
                        result = false;
                    }
                }
            }
        }
        else
        {
            index = System.Array.FindIndex(cells, e => e.itemCount == 0);
            if (index == -1)
            {
                result = false;
            }
        }
        return result;
    }

    public bool TryAddItem(string code)
    {
        bool result = true;
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == code);
        if (index != -1)
        {
            if (cells[index].data.maxCount != -1)
            {
                if (cells[index].itemCount >= cells[index].data.maxCount)
                {
                    result = false;
                }
            }
        }
        else
        {
            index = System.Array.FindIndex(cells, e => e.itemCount == 0);
            if (index == -1)
            {
                result = false;
            }
        }
        return result;
    }

    public int ItemCount(string code)
    {
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == code);
        if (index != -1)
        {
            return cells[index].itemCount;
        }
        else
        {
            return -1;
        }
    }

    public bool HasItem(string code)
    {
        return System.Array.FindIndex(cells, e => e.data.itemCode == code) != -1;
    }

    public int FindIndex(string code)
    {
        return System.Array.FindIndex(cells, e => e.data.itemCode == code);
    }
}
