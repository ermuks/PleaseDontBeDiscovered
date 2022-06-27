using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public int gold;
    private ItemCell[] cells;
    public ItemCell this[int index] => cells[index];
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
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == code);
        if (index != -1)
        {
            cells[index].itemCount += count;
        }
        else
        {
            index = System.Array.FindIndex(cells, e => e.itemCount == 0);
            if (index != -1)
            {
                cells[index].data = ItemManager.GetItem(code);
                cells[index].itemCount = count;
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

    public void UseItem(int index, int count)
    {
        if (cells[index].itemCount >= 0)
        {
            cells[index].data.UseItem();
            Remove(index, count);
        }
    }

    public void ChangeItem(string origin, string code, bool useable)
    {
        int index = System.Array.FindIndex(cells, e => e.data.itemCode == origin);
        if (index != -1)
        {
            if (!useable) Remove(origin, 1);
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
}
