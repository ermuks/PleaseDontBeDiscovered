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
        ItemManager.AddItem(
            new ItemData(
                "0000", 
                Strings.GetString(StringKey.ItemFish),
                Strings.GetString(StringKey.ItemFishDescript), 
                3, 
                "Item :: Fish"));

        ItemManager.AddItem(new ItemData("0001", Strings.GetString(StringKey.ItemGrilledFish), Strings.GetString(StringKey.ItemGrilledFishDescript), 3, 
            "Item :: GrilledFish"));

        ItemManager.AddItem(new ItemData("0002", Strings.GetString(StringKey.ItemEmptyBottle), Strings.GetString(StringKey.ItemEmptyBottleDescript), 3, 
            "Item :: EmptyBottle"));

        ItemManager.AddItem(new ItemData("0003", Strings.GetString(StringKey.ItemFullBottle), Strings.GetString(StringKey.ItemFullBottleDescript), 3, 
            "Item :: FullBottle"));

        ItemManager.AddItem(new ItemData("0004", Strings.GetString(StringKey.ItemHandWarmer), Strings.GetString(StringKey.ItemHandWarmerDescript), 1,
            "Item :: HandWarmer"));

        ItemManager.AddItem(new ItemData("0005", Strings.GetString(StringKey.ItemUsingHandWarmer), Strings.GetString(StringKey.ItemUsingHandWarmerDescript), 1, 
            "Item :: UsingHandWarmer"));

        ItemManager.AddItem(new ItemData("0006", Strings.GetString(StringKey.ItemColdHandWarmer), Strings.GetString(StringKey.ItemColdHandWarmerDescript), 1,
            "Item :: ColdHandWarmer"));

        ItemManager.AddItem(new ItemData("0007", Strings.GetString(StringKey.ItemWood), Strings.GetString(StringKey.ItemWoodDescript), 10, 
            "Item :: Wood"));
    }
}
