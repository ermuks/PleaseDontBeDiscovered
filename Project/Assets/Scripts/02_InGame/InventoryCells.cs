using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryCells : EventTrigger
{
    private ItemCell cell;
    [SerializeField] private Image imgItemImage;
    [SerializeField] private TMP_Text txtItemCount;

    public void Init(ItemCell cell)
    {
        this.cell = cell;
        Refresh();
    }

    public void Refresh()
    {
        imgItemImage.gameObject.SetActive(cell.itemCount > 0);
        txtItemCount.gameObject.SetActive(cell.itemCount > 0);
        if (cell.itemCount > 0)
        {
            imgItemImage.sprite = cell.data.itemImage;
            txtItemCount.text = $"{cell.itemCount}";
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(1f, 1f, .5f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {

        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }
}
