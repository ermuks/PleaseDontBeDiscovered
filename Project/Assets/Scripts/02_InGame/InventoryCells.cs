using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryCells : EventTrigger
{
    public bool isMine;
    public int index;

    private ItemCell cell;
    [SerializeField] private Image imgItemImage;
    [SerializeField] private TMP_Text txtItemCount;

    public void Init(ItemCell cell, int index, bool isMine)
    {
        this.cell = cell;
        this.index = index;
        this.isMine = isMine;
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

    public void ResetEvent()
    {
        GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (cell.itemCount > 0)
        {
            EventManager.SendEvent("InventoryUI :: ShowItemInfo", cell.data);
        }
        GetComponent<Image>().color = new Color(1f, 1f, .5f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        EventManager.SendEvent("InventoryUI :: ClearItemInfo");
        GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isMine)
            {
                if ((bool)EventManager.GetData("InventoryUI >> TryAddItemMulty", cell.data.itemCode)) 
                {
                    Debug.Log("A");
                    string code = cell.data.itemCode;
                    EventManager.SendEvent("InventoryUI :: AddItem", code);
                    EventManager.SendEvent("Inventory :: RemoveIndex", index, 1);
                    EventManager.SendEvent("InventoryUI :: SendData");
                }
                else
                {
                    Debug.Log("B");
                    EventManager.SendEvent("InGameUI :: CreateMessage", Strings.GetString(StringKey.InGameMessageCantMoveItem));
                }
            }
            else
            {
                if ((bool)EventManager.GetData("Inventory >> TryAddItem", cell.data.itemCode))
                {
                    Debug.Log("C");
                    string code = cell.data.itemCode;
                    EventManager.SendEvent("Inventory :: AddItem", code, 1);
                    EventManager.SendEvent("InventoryUI :: Remove", index);
                    EventManager.SendEvent("InventoryUI :: SendData");
                }
                else
                {
                    Debug.Log("D");
                    EventManager.SendEvent("InGameUI :: CreateMessage", Strings.GetString(StringKey.InGameMessageCantMoveItem));
                }
            }
            EventManager.SendEvent("Inventory :: Refresh");
            EventManager.SendEvent("InventoryUI :: Refresh");
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (cell.itemCount <= 0) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EventManager.SendEvent("InventoryUI :: BeginDrag", cell.data.itemImage);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EventManager.SendEvent("InventoryUI :: EndDrag");
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (cell.itemCount <= 0) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EventManager.SendEvent("InventoryUI :: FollowItem", eventData.position);
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            InventoryCells dragCell = eventData.pointerDrag.GetComponent<InventoryCells>();
            if (dragCell != null)
            {
                if (dragCell.cell.itemCount > 0)
                {
                    ItemCell newCell = new ItemCell();
                    if (!dragCell.isMine && !isMine)
                    {
                        EventManager.SendEvent("InventoryUI :: SwapItem", dragCell.index, index);
                        EventManager.SendEvent("InventoryUI :: SendData");
                    }
                    else if (dragCell.isMine && isMine)
                    {
                        if ((bool)EventManager.GetData("Inventory >> TryAddItem", dragCell.cell.data.itemCode))
                        {
                            EventManager.SendEvent("Inventory :: SwapItem", dragCell.index, index);
                            EventManager.SendEvent("Inventory :: Refresh");
                        }
                        else
                        {
                            EventManager.SendEvent("InGameUI :: CreateMessage", Strings.GetString(StringKey.InGameMessageCantMoveItem));
                        }
                    }
                    else if (dragCell.isMine != isMine)
                    {
                        newCell.SetCell(dragCell.cell);
                        dragCell.cell.SetCell(cell);
                        cell.SetCell(newCell);
                        EventManager.SendEvent("Inventory :: Refresh");
                        EventManager.SendEvent("InventoryUI :: Refresh");
                        EventManager.SendEvent("InventoryUI :: SendData");
                    }
                }
            }
        }
    }
}
