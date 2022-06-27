using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCellManager : MonoBehaviour
{
    [SerializeField] private Image imgItemImage;
    [SerializeField] private TMP_Text txtItemCount;

    public ItemCell cell;
    int myIndex = -1;

    public void Init(int index)
    {
        myIndex = index;
    }

    public void SetItem(ItemCell cell)
    {
        this.cell = cell;
    }

    public void Refresh()
    {
        if (cell.itemCount <= 0)
        {
            imgItemImage.gameObject.SetActive(false);
            txtItemCount.text = "";
        }
        else
        {
            imgItemImage.gameObject.SetActive(true);
            imgItemImage.sprite = cell.data.itemImage;
            txtItemCount.text = cell.itemCount.ToString();
        }
    }
}
