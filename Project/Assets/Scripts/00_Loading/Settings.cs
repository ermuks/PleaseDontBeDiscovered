using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeySettings
{
    ForwardKey = 0,
    BackwardKey,
    RightKey,
    LeftKey,
    JumpKey,
    SitKey,
    RunKey,
    WalkKey,

    AroundKey,
    WatchNextPlayer,
    WatchPreviewPlayer,

    UseItem1,
    UseItem2,
    Work,
    CancelWork,

    KeyCount,
}

public class Settings : MonoBehaviour
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

    public bool isFullScreen;
    public bool isAntiAliasing;
    public bool isVSync;

    private Dictionary<KeySettings, KeyCode> keys = new Dictionary<KeySettings, KeyCode>();

    private KeyCode[] defaultKeys = {
        KeyCode.W,
        KeyCode.S,
        KeyCode.D,
        KeyCode.A,
        KeyCode.Space,
        KeyCode.C,
        KeyCode.LeftShift,
        KeyCode.LeftControl,

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
        for (int i = 0; i < (int)KeySettings.KeyCount; i++)
        {
            keys.Add((KeySettings)i, defaultKeys[i]);
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
            Screen.SetResolution(width, height, false);
        }
    }
}
