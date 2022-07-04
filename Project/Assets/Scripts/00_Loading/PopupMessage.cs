using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupMessage : MonoBehaviour
{
    [SerializeField] private GameObject areaMessagePopupUI;
    [SerializeField] private TMP_Text txtContent;
    [SerializeField] private Button btnSubmit;

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
        if (Input.GetKeyDown(KeyCode.Return) && areaMessagePopupUI.activeSelf)
        {
            areaMessagePopupUI.SetActive(false);
        }
    }
}
