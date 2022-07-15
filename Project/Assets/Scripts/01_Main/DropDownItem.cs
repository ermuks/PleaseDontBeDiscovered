using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownItem : MonoBehaviour
{
    public void SetSize(int width, int height)
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Settings.instance.SetWindow(width, height);
        });
    }
}
