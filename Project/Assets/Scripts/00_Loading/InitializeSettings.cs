using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeSettings : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objects;

    private void Awake()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            DontDestroyOnLoad(objects[i]);
        }
        ItemManager.AddItem(new ItemData("0000", Strings.GetString(StringKey.ItemFish), Strings.GetString(StringKey.ItemFishDescript), "Item :: Fish"));
        ItemManager.AddItem(new ItemData("0001", Strings.GetString(StringKey.ItemGrilledFish), Strings.GetString(StringKey.ItemGrilledFishDescript), "Item :: GrilledFish"));
        ItemManager.AddItem(new ItemData("0002", Strings.GetString(StringKey.ItemEmptyBottle), Strings.GetString(StringKey.ItemEmptyBottleDescript), "Item :: EmptyBottle"));
        ItemManager.AddItem(new ItemData("0003", Strings.GetString(StringKey.ItemFullBottle), Strings.GetString(StringKey.ItemFullBottleDescript), "Item :: FullBottle"));
        ItemManager.AddItem(new ItemData("0004", Strings.GetString(StringKey.ItemHandWarmer), Strings.GetString(StringKey.ItemHandWarmerDescript), "Item :: HandWarmer"));
        ItemManager.AddItem(new ItemData("0005", Strings.GetString(StringKey.ItemUsingHandWarmer), Strings.GetString(StringKey.ItemUsingHandWarmerDescript), "Item :: UsingHandWarmer"));
        ItemManager.AddItem(new ItemData("0006", Strings.GetString(StringKey.ItemColdHandWarmer), Strings.GetString(StringKey.ItemColdHandWarmerDescript), "Item :: ColdHandWarmer"));
        ItemManager.AddItem(new ItemData("0007", Strings.GetString(StringKey.ItemWood), Strings.GetString(StringKey.ItemWoodDescript), "Item :: Wood"));
    }
}
