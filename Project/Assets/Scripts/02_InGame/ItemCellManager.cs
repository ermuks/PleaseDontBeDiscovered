using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCellManager : MonoBehaviour
{
    [SerializeField] private Image imgItemImage;
    [SerializeField] private Image imgItemCover;
    [SerializeField] private TMP_Text txtItemCount;

    public ItemCell cell;
    private int myIndex = -1;

    private float t = .0f;
    private Coroutine timer = null;

    public void UseHandWarmer(float lifeTime)
    {
        imgItemCover.gameObject.SetActive(true);
        t = .0f;
        if (timer != null) StopCoroutine(timer);
        timer = StartCoroutine(HandWarmer(lifeTime));
    }

    private IEnumerator HandWarmer(float lifeTime)
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
            imgItemCover.fillAmount = t / lifeTime;
            if (t >= lifeTime)
            {
                EventManager.SendEvent("Inventory :: Change", "0005", "0006");
                imgItemCover.gameObject.SetActive(false);
                break;
            }
        }
    }

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
