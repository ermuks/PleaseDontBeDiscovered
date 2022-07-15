using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommonUIManager : MonoBehaviour
{
    [SerializeField] private GameObject areaMessagePopupUI;
    [SerializeField] private TMP_Text txtContent;
    [SerializeField] private Button btnSubmit;

    [SerializeField] private GameObject areaSettings;
    [SerializeField] private Button[] btnTabs;

    private void Awake()
    {
        areaMessagePopupUI.SetActive(false);
        EventManager.AddEvent("PopupMessage", (p) =>
        {
            txtContent.text = (string)p[0];
            areaMessagePopupUI.SetActive(true);
        });

        btnSubmit.onClick.AddListener(() =>
        {
            areaMessagePopupUI.SetActive(false);
        });
    }

    private void Update()
    {
        if (areaMessagePopupUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                areaMessagePopupUI.SetActive(false);
            }
        }
    }
}
