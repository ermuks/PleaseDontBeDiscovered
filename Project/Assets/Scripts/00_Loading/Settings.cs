using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public enum KeySettings
{
    ForwardKey = 0,
    BackwardKey,
    RightKey,
    LeftKey,

    JumpKey,
    RunKey,
    WalkKey,

    MinimapKey,
    CCTVKey,

    AroundKey,
    WatchNextPlayer,
    WatchPrevPlayer,

    UseItem1,
    UseItem2,
    Work,
    CancelWork,

    KeyCount,
}

public class Settings : MonoBehaviourPunCallbacks
{
    private static Settings _instance;
    public static Settings instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Settings>();
            return _instance;
        }
    }
    public bool isDebug = true;
    public bool isMurderMode = true;
    [SerializeField] private Button[] btnTabs;
    [SerializeField] private GameObject[] areaTabs;

    [SerializeField] private Button btnExitSettings;
    [SerializeField] private GameObject areaDebugUI;
    [SerializeField] private GameObject areaSettings;

    [SerializeField] private Toggle tglVSync;
    [SerializeField] private Toggle tglAntiAliasing;
    [SerializeField] private Toggle tglFullScreen;
    [SerializeField] private Button btnToggleDropDown;
    [SerializeField] private TMP_Text txtWindowSize;
    [SerializeField] private GameObject areaWindowSizeList;

    [SerializeField] private GameObject areaKeyboard;

    private List<int> indexList
    {
        get
        {
            List<int> list = new List<int>();
            if (PhotonNetwork.InRoom)
            {
                foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    if (player != PhotonNetwork.LocalPlayer)
                    {
                        int index = (int)player.CustomProperties["color"];
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                }
            }
            return list;
        }
    }
    [SerializeField] private Transform colorSelectParent;
    [SerializeField] private Transform controlParent;

    private List<ColorSelectButton> btnColorList = new List<ColorSelectButton>();
    private List<ControllerList> btnControllerList = new List<ControllerList>();

    private Dictionary<KeySettings, KeyCode> keys = new Dictionary<KeySettings, KeyCode>();

    private KeyCode[] defaultKeys = {
        KeyCode.W,
        KeyCode.S,
        KeyCode.D,
        KeyCode.A,

        KeyCode.Space,
        KeyCode.LeftShift,
        KeyCode.LeftControl,

        KeyCode.Tab,
        KeyCode.C,

        KeyCode.LeftAlt,
        KeyCode.PageUp,
        KeyCode.PageDown,

        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.F,
        KeyCode.Escape
    };

    public KeyCode GetKey(KeySettings type) => keys[type];

    private void Awake()
    {
        GameObject colorPrefab = Resources.Load<GameObject>("Prefabs/UI/ColorSelectButton");
        GameObject controlPrefab = Resources.Load<GameObject>("Prefabs/UI/ControllerList");
        AddEvent();
        areaSettings.SetActive(false);
        areaDebugUI.SetActive(isDebug);
        for (int i = 0; i < (int)KeySettings.KeyCount; i++)
        {
            keys.Add((KeySettings)i, defaultKeys[i]);
            ControllerList control = Instantiate(controlPrefab, controlParent).GetComponent<ControllerList>();
            control.Init((KeySettings)i, defaultKeys[i]);
            btnControllerList.Add(control);
        }
        for (int i = 0; i < PlayerData.colors.Length; i++)
        {
            ColorSelectButton colorButton = Instantiate(colorPrefab, colorSelectParent).GetComponent<ColorSelectButton>();
            colorButton.Init(i);
            btnColorList.Add(colorButton);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            foreach (var item in Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties)
            {
                Debug.Log($"{item.Key} :: {item.Value}");
            }
            EventManager.SendEvent("InGameUI :: CreateMessage", "디버그용 버튼 (F3)");
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            foreach (var p in Photon.Pun.PhotonNetwork.CurrentRoom.Players)
            {
                foreach (var pr in p.Value.CustomProperties)
                {
                    Debug.Log($"{p.Value.NickName} :: {pr.Key} :: {pr.Value}");
                }
            }
            EventManager.SendEvent("InGameUI :: CreateMessage", "디버그용 버튼 (F4)");
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            foreach (var p in Photon.Pun.PhotonNetwork.CurrentRoom.CustomProperties)
            {
                Debug.Log($"{p.Key} :: {p.Value}");
            }
            EventManager.SendEvent("InGameUI :: CreateMessage", "디버그용 버튼 (F5)");
        }
    }

    public override void OnJoinedRoom()
    {
        int colorIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["color"];
        if (indexList.FindIndex(e => e == colorIndex) != -1)
        {
            SetSelectRandomColor();
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        PlayerColorRefresh();
    }

    private void AddEvent()
    {
        tglVSync.onValueChanged.AddListener((v) => SetVSync(v));
        tglAntiAliasing.onValueChanged.AddListener((v) => SetAntiAliasing(v));
        tglFullScreen.onValueChanged.AddListener((v) => SetFullScreen(v));
        btnExitSettings.onClick.AddListener(() => CloseSettings());
        btnToggleDropDown.onClick.AddListener(() => areaWindowSizeList.SetActive(!areaWindowSizeList.activeSelf));
        for (int i = 0; i < btnTabs.Length; i++)
        {
            int index = i;
            btnTabs[i].onClick.AddListener(() =>
            {
                for (int j = 0; j < areaTabs.Length; j++)
                {
                    OpenSettings(index);
                }
            });
        }
    }

    public void OpenKeyboard(KeySettings key)
    {
        areaKeyboard.SetActive(true);
        areaKeyboard.GetComponent<KeyboardUI>().Open(key);
    }

    public void CloseKeyboard()
    {
        areaKeyboard.SetActive(false);
    }

    public void SetKey(KeySettings key, KeyCode code)
    {
        keys[key] = code;
        KeyRefresh();
    }

    public void SetKey(KeySettings key, char character)
    {
        keys[key] = (KeyCode)character;
        KeyRefresh();
    }

    private void KeyRefresh()
    {
        for (int i = 0; i < btnControllerList.Count; i++)
        {
            btnControllerList[i].Refresh();
        }
    }

    public void SetPlayerColor(int index)
    {
        var properties = PhotonNetwork.LocalPlayer.CustomProperties;
        properties["color"] = index;
        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        PlayerColorRefresh();
    }

    public void PlayerColorRefresh()
    {
        for (int i = 0; i < btnColorList.Count; i++)
        {
            bool alreadySelected = indexList.FindIndex(e => e == i) != -1;
            btnColorList[i].Checked(false);
            btnColorList[i].Interactable(!alreadySelected);
        }
        int myIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["color"];
        btnColorList[myIndex].Checked(true);
        btnColorList[myIndex].Interactable(true);
    }

    public void SetSelectRandomColor()
    {
        bool find = false;
        while (!find)
        {
            int colorIndex = Random.Range(0, PlayerData.colors.Length);
            if (indexList.FindIndex(e => e == colorIndex) == -1)
            {
                SetPlayerColor(colorIndex);
                find = true;
            }
        }
    }

    public void OpenSettings(int index = 0)
    {
        areaSettings.SetActive(true);
        for (int j = 0; j < areaTabs.Length; j++)
        {
            areaTabs[j].SetActive(index == j);
        }
        PlayerColorRefresh();
    }

    public void CloseSettings()
    {
        areaSettings.SetActive(false);
    }

    public void CloseDropDownUI()
    {
        areaWindowSizeList.SetActive(false);
    }

    public void SetAntiAliasing(bool value)
    {
        QualitySettings.antiAliasing = value ? 3 : 0;
    }

    public void SetVSync(bool value)
    {
        QualitySettings.vSyncCount = value ? 1 : 0;
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
    }

    public void SetWindow(int width, int height)
    {
        if (!Screen.fullScreen)
        {
            txtWindowSize.text = $"{width} * {height}";
            Screen.SetResolution(width, height, false);
        }
    }
}
