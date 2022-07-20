using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class ColorSelectButton : MonoBehaviour
{
    [SerializeField] private Image imgColor;
    [SerializeField] private GameObject areaChecked;
    [SerializeField] private GameObject areaInteractable;
    
    Button me;

    public bool interactable;
    public bool isChecked;
    public int index = -1;

    public void Init(int index)
    {
        interactable = true;
        isChecked = false;
        imgColor.color = PlayerData.GetColor(index);
        this.index = index;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (interactable)
            {
                Settings.instance.SetPlayerColor(index);
                PlayerPrefs.SetInt("color", index);
            }
        });
    }

    public void Interactable(bool value)
    {
        interactable = value;
        Refresh();
    }

    public void Checked(bool value)
    {
        isChecked = value;
        Refresh();
    }

    public void Refresh()
    {
        areaChecked.SetActive(isChecked);
        areaInteractable.SetActive(!interactable);
    }
}
