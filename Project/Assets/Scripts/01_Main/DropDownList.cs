using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownList : MonoBehaviour
{
    private GameObject dropDownPrefab;

    private void Awake()
    {
        dropDownPrefab = Resources.Load<GameObject>("Prefabs/UI/DropDownItem");

        CreateItem(1920, 1080);
        CreateItem(1600, 900);
        CreateItem(1360, 765);
        CreateItem(1280, 720);
        CreateItem(1024, 768);
        CreateItem(1024, 576);
        CreateItem(800, 600);
        CreateItem(800, 450);
    }

    private void CreateItem(int width, int height)
    {
        DropDownItem item = Instantiate(dropDownPrefab, transform).GetComponent<DropDownItem>();
        item.SetSize(width, height);
    }
}
