using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropDownItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtContent;
    public void SetSize(int width, int height)
    {
        txtContent.text = $"{width} * {height}";
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Settings.instance.SetWindow(width, height);
            Settings.instance.CloseDropDownUI();
        });
    }
}
