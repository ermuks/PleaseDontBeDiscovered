using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class NetworkManager_Main : MonoBehaviourPunCallbacks
{
    [Header("UI Settings")]
    #region UI
    [SerializeField] private GameObject areaInputNickname;
    [SerializeField] private GameObject areaRoomList;
    [SerializeField] private GameObject areaCreateRoom;
    [SerializeField] private GameObject areaRoomUI;
    [SerializeField] private GameObject areaMasterMenu;

    [Header("** Create Room UI **")]
    [SerializeField] private TMP_InputField inputRoomTitle;

    [Space(16)]
    [SerializeField] private TMP_InputField inputRoomMaxPlayer;
    [SerializeField] private TMP_Text txtRoomMaxPlayer;
    [SerializeField] private Button btnRoomMaxPlayer;
    [SerializeField] private Slider sliderRoomMaxPlayer;

    [Space(8)]
    [SerializeField] private TMP_InputField inputMurderCount;
    [SerializeField] private TMP_Text txtMurderCount;
    [SerializeField] private Button btnMurderCount;
    [SerializeField] private Slider sliderMurderCount;

    [Space(8)]
    [SerializeField] private TMP_InputField inputMoveSpeed;
    [SerializeField] private TMP_Text txtMoveSpeed;
    [SerializeField] private Button btnMoveSpeed;
    [SerializeField] private Slider sliderMoveSpeed;

    [Space(8)]
    [SerializeField] private TMP_InputField inputKillCooldown;
    [SerializeField] private TMP_Text txtKillCooldown;
    [SerializeField] private Button btnKillCooldown;
    [SerializeField] private Slider sliderKillCooldown;

    [Space(8)]
    [SerializeField] private Toggle tglNickname;
    [SerializeField] private Toggle tglFallingDamage;
    [SerializeField] private Toggle tglStartItem;
    [SerializeField] private Toggle tglRunable;

    [Space(16)]
    [SerializeField] private Button btnOpenCreateRoom;
    [SerializeField] private Button btnRandomJoinRoom;
    [SerializeField] private Button btnCreateRoom;
    [SerializeField] private Button btnCreateCancel;

    [Header("** Inner Room UI **")]
    [SerializeField] private Button btnStart;
    [SerializeField] private Button btnReady;
    [SerializeField] private Button btnCancel;
    [SerializeField] private Button btnExit;

    private Player selectPlayer;
    [SerializeField] private TMP_Text txtTargetNickname;
    [SerializeField] private Button btnExplusion;
    [SerializeField] private Button btnBlackList;
    #endregion

    [Header("Nickname Settings")]
    #region Nickname Settings
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private Button buttonSubmit;
    #endregion

    [Header("Room Settings")]
    #region Room Settings
    [SerializeField]
    private Transform roomListParent;
    private GameObject roomListPrefab;

    private Dictionary<string, RoomInfo> rooms = new Dictionary<string, RoomInfo>();
    private List<RoomListItem> roomListItems = new List<RoomListItem>();
    #endregion

    [Header("Player Settings")]
    #region Player Settings
    [SerializeField]
    private Transform playerListParent;
    private GameObject playerListPrefab;

    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
    #endregion

    #region string Data
    readonly private string[] randomTitle = { "아무나 들어와~~", "처음 하는 사람만!!", "ㅋㅋㅋㅋ 너무 재밌네", "빨리 ㄱㄱ", "여기로 들어와!!" };
    #endregion

    #region Unity Method
    // ***** Awake ***** //
    private void Awake()
    {
        LoadPrefab();
        CheckRoomLobby();
        UIAddEvents();

        areaMasterMenu.SetActive(false);
        EventManager.AddEvent("MainUI :: OpenMasterMenu", (p) =>
        {
            areaMasterMenu.SetActive(true);
            areaMasterMenu.transform.position = (Vector2)p[0];
            selectPlayer = (Player)p[1];
            txtTargetNickname.text = selectPlayer.NickName;
        });
        EventManager.AddEvent("MainUI :: CloseMasterMenu", (p) =>
        {
            areaMasterMenu.SetActive(true);
        });
    }

    private void LateUpdate()
    {
        if (areaMasterMenu.activeInHierarchy)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Escape))
            {
                areaMasterMenu.SetActive(false);
            }
        }
    }

    #endregion

    #region Photon Method
    // ***** OnConnectedToMaster ***** //
    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    // ***** OnJoinedLobby ***** //
    public override void OnJoinedLobby()
    {
        EventManager.SendEvent("Chatting :: Hide");
    }

    // ***** OnJoinedRoom ***** //
    public override void OnJoinedRoom()
    {
        areaRoomList.SetActive(false);
        areaCreateRoom.SetActive(false);
        areaRoomUI.SetActive(true);
        SetPlayerProperties("isReady", false, "isMurder", false, "isDead", false);
        for (int i = 0; i < playerListItems.Count; i++)
        {
            Destroy(playerListItems[i].gameObject);
        }
        playerListItems.Clear();
        for (int i = 0; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            PlayerListItem item = Instantiate(playerListPrefab, playerListParent).GetComponent<PlayerListItem>();
            playerListItems.Add(item);
        }

        CheckIsMasterClient();

        EventManager.SendEvent("Chatting :: Show");

        players.Clear();
        foreach (var item in PhotonNetwork.CurrentRoom.Players)
        {
            players.Add(item.Value.NickName, item.Value);
        }
        RefreshPlayerList();
    }

    // ***** OnLeftRoom ***** //
    public override void OnLeftRoom()
    {
        areaRoomList.SetActive(true);
        areaRoomUI.SetActive(false);
        SetPlayerProperties("isReady", false);

        players.Clear();
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    // ***** OnPlayerEnteredRoom ***** //
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        players.Add(newPlayer.NickName, newPlayer);
        RefreshPlayerList();
    }

    // ***** OnPlayerLeftRoom ***** //
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        players.Remove(otherPlayer.NickName);
        CheckIsMasterClient();
        RefreshPlayerList();
    }

    // ***** OnPlayerPropertiesUpdate ***** //
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (players.ContainsKey(targetPlayer.NickName))
        {
            players[targetPlayer.NickName].CustomProperties = changedProps;
        }
        RefreshPlayerList();
    }



    // ***** OnRoomListUpdate ***** //
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var item in roomList)
        {
            if (item.RemovedFromList || !item.IsOpen || !item.IsVisible)
            {
                rooms.Remove(item.Name);
            }
            else
            {
                if (rooms.ContainsKey(item.Name))
                {
                    rooms[item.Name] = item;
                }
                else
                {
                    rooms.Add(item.Name, item);
                }
            }
        }
        RefreshRoomList();
    }

    // ***** OnRoomPropertiesUpdate ***** //
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("isStart"))
        {
            if ((bool)propertiesThatChanged["isStart"])
            {
                var properties = PhotonNetwork.LocalPlayer.CustomProperties;
                properties["isReady"] = false;
                PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
                EventManager.SendEvent("OpenScene", "InGame");
            }
        }
    }

    // ***** OnJoinRoomFailed ***** //
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        EventManager.SendEvent("PopupMessage", $"방 입장에 실패했습니다.\nError Code : {returnCode}");
    }

    // ***** OnJoinRandomFailed ***** //
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        EventManager.SendEvent("PopupMessage", $"랜덤 입장에 실패했습니다.\nError Code : {returnCode}");
    }
    #endregion Photon Method

    #region Custom Method

    private void LoadPrefab()
    {
        roomListPrefab = Resources.Load<GameObject>("Prefabs/UI/RoomListItem");
        playerListPrefab = Resources.Load<GameObject>("Prefabs/UI/PlayerListItem");
    }

    private void CheckRoomLobby()
    {
        if (PhotonNetwork.InRoom)
        {
            areaInputNickname.SetActive(false);
            areaRoomList.SetActive(false);
            areaCreateRoom.SetActive(false);
            areaRoomUI.SetActive(true);
            OnJoinedRoom();
        }
        else
        {
            if (PhotonNetwork.InLobby)
            {
                areaInputNickname.SetActive(false);
                areaRoomList.SetActive(true);
                areaCreateRoom.SetActive(false);
                areaRoomUI.SetActive(false);
            }
            else
            {
                areaInputNickname.SetActive(true);
                areaRoomList.SetActive(false);
                areaCreateRoom.SetActive(false);
                areaRoomUI.SetActive(false);
                PhotonNetwork.JoinLobby();
            }
        }
    }

    private void UIAddEvents()
    {
        btnOpenCreateRoom.onClick.AddListener(() =>
        {
            inputRoomTitle.text = randomTitle[Random.Range(0, randomTitle.Length)];

            inputRoomMaxPlayer.gameObject.SetActive(false);
            inputMurderCount.gameObject.SetActive(false);
            inputMoveSpeed.gameObject.SetActive(false);

            sliderRoomMaxPlayer.value = 8;
            sliderMurderCount.value = 2;
            sliderMoveSpeed.value = 5;

            txtRoomMaxPlayer.text = "8";
            txtMurderCount.text = "2";
            txtMoveSpeed.text = "5";

            areaRoomList.SetActive(false);
            areaCreateRoom.SetActive(true);
        });
        btnRandomJoinRoom.onClick.AddListener(() =>
        {
            PhotonNetwork.JoinRandomRoom();
        });
        btnCreateRoom.onClick.AddListener(() =>
        {
            CreateRoom();
        });
        btnCreateCancel.onClick.AddListener(() =>
        {
            areaRoomList.SetActive(true);
            areaCreateRoom.SetActive(false);
        });

        btnStart.onClick.AddListener(() =>
        {
            if (Settings.instance.isDebug)
            {
                var p = PhotonNetwork.LocalPlayer.CustomProperties;
                p["isMurder"] = Settings.instance.isMurderMode;
                PhotonNetwork.LocalPlayer.SetCustomProperties(p);
                PhotonNetwork.CurrentRoom.IsOpen = false;
                SetRoomProperties("isStart", true);
            }
            else
            {
                OnStart();
            }
        });
        btnReady.onClick.AddListener(() =>
        {
            SetPlayerProperties("isReady", true);
            btnReady.gameObject.SetActive(false);
            btnCancel.gameObject.SetActive(true);
        });
        btnCancel.onClick.AddListener(() =>
        {
            SetPlayerProperties("isReady", false);
            btnReady.gameObject.SetActive(true);
            btnCancel.gameObject.SetActive(false);
        });
        btnExit.onClick.AddListener(() =>
        {
            PhotonNetwork.LeaveRoom();
        });
        inputNickname.onSubmit.AddListener((value) =>
        {
            SetNickname(value);
        });

        buttonSubmit.onClick.AddListener(() =>
        {
            SetNickname(inputNickname.text);
        });

        inputRoomMaxPlayer.onValueChanged.AddListener((value) =>
        {
            if (int.TryParse(inputRoomMaxPlayer.text, out int count))
            {
                if (int.Parse(inputRoomMaxPlayer.text) < 4) inputRoomMaxPlayer.text = "4";
                if (int.Parse(inputRoomMaxPlayer.text) > 16) inputRoomMaxPlayer.text = "16";
            }
            else
            {
                inputRoomMaxPlayer.text = "4";
            }
            sliderMurderCount.maxValue = int.Parse(inputRoomMaxPlayer.text) / 2 - 1;
        });
        inputMurderCount.onValueChanged.AddListener((value) =>
        {
            if (int.TryParse(inputMurderCount.text, out int count))
            {
                if (int.Parse(inputMurderCount.text) < 1) inputRoomMaxPlayer.text = "1";
                if (int.Parse(inputMurderCount.text) > sliderMurderCount.maxValue) inputRoomMaxPlayer.text = sliderMurderCount.maxValue.ToString();
            }
            else
            {
                inputMurderCount.text = "4";
            }
        });
        inputMoveSpeed.onValueChanged.AddListener((value) =>
        {
            if (int.TryParse(inputMoveSpeed.text, out int count))
            {
                if (int.Parse(inputMoveSpeed.text) < 3) inputMoveSpeed.text = "3";
                if (int.Parse(inputMoveSpeed.text) > 20) inputMoveSpeed.text = "20";
            }
            else
            {
                inputRoomMaxPlayer.text = "3";
            }
        });
        inputKillCooldown.onValueChanged.AddListener((value) =>
        {
            if (int.TryParse(inputKillCooldown.text, out int count))
            {
                if (int.Parse(inputKillCooldown.text) < 5) inputKillCooldown.text = "5";
                if (int.Parse(inputKillCooldown.text) > 300) inputKillCooldown.text = "300";
            }
            else
            {
                inputRoomMaxPlayer.text = "3";
            }
        });

        inputRoomMaxPlayer.onSubmit.AddListener((value) =>
        {
            sliderRoomMaxPlayer.value = int.Parse(inputRoomMaxPlayer.text);
        });
        inputMurderCount.onSubmit.AddListener((value) =>
        {
            if (int.TryParse(inputMurderCount.text, out int count))
            {
                int maxPlayer = int.Parse(inputRoomMaxPlayer.text);
                if (count >= 1 && count < maxPlayer / 2)
                {
                    sliderMurderCount.value = count;
                }
                else
                {
                    if (count >= maxPlayer / 2)
                    {
                        EventManager.SendEvent("PopupMessage", $"킬러 수가 너무 많습니다.\n최대 인원 : {maxPlayer / 2 - 1}");
                    }
                    if (count < 1)
                    {
                        EventManager.SendEvent("PopupMessage", $"킬러 수가 너무 적습니다.\n최소 인원 : 1");
                    }
                }
            }
        });
        inputMoveSpeed.onSubmit.AddListener((value) =>
        {
            sliderMoveSpeed.value = float.Parse(value);
        });
        inputKillCooldown.onSubmit.AddListener((value) =>
        {
            sliderKillCooldown.value = float.Parse(value);
        });

        inputRoomMaxPlayer.onEndEdit.AddListener((value) => inputRoomMaxPlayer.gameObject.SetActive(false));
        inputMurderCount.onEndEdit.AddListener((value) => inputMurderCount.gameObject.SetActive(false));
        inputMoveSpeed.onEndEdit.AddListener((value) => inputMoveSpeed.gameObject.SetActive(false));
        inputKillCooldown.onEndEdit.AddListener((value) => inputKillCooldown.gameObject.SetActive(false));

        sliderRoomMaxPlayer.onValueChanged.AddListener((value) =>
        {
            txtRoomMaxPlayer.text = $"{value:0}";
            sliderMurderCount.maxValue = (int)(value) / 2 - 1;
        });
        sliderMurderCount.onValueChanged.AddListener((value) =>
        {
            txtMurderCount.text = $"{value:0}";
        });
        sliderMoveSpeed.onValueChanged.AddListener((value) =>
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                value = (int)value;
                sliderMoveSpeed.value = value;
            }
            txtMoveSpeed.text = $"{value:0.0}";
        });
        sliderKillCooldown.onValueChanged.AddListener((value) =>
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                value = (int)(value / 5) * 5;
                sliderKillCooldown.value = value;
            }
            txtKillCooldown.text = $"{value:0}";
        });

        btnRoomMaxPlayer.onClick.AddListener(() =>
        {
            inputRoomMaxPlayer.gameObject.SetActive(true);
            inputRoomMaxPlayer.ActivateInputField();
            inputRoomMaxPlayer.text = sliderRoomMaxPlayer.value.ToString();
        });
        btnMurderCount.onClick.AddListener(() =>
        {
            inputMurderCount.gameObject.SetActive(true);
            inputMurderCount.ActivateInputField();
            inputMurderCount.text = sliderMurderCount.value.ToString();
        });
        btnMoveSpeed.onClick.AddListener(() =>
        {
            inputMoveSpeed.gameObject.SetActive(true);
            inputMoveSpeed.ActivateInputField();
            inputMoveSpeed.text = sliderMoveSpeed.value.ToString();
        });
        btnKillCooldown.onClick.AddListener(() =>
        {
            inputKillCooldown.gameObject.SetActive(true);
            inputKillCooldown.ActivateInputField();
            inputKillCooldown.text = sliderKillCooldown.value.ToString();
        });

        btnExplusion.onClick.AddListener(() =>
        {
            
        });
        btnBlackList.onClick.AddListener(() =>
        {
            
        });
    }
    // ***** CreateRoom ***** //
    private void CreateRoom()
    {
        string title = inputRoomTitle.text;
        byte playerCount = (byte)sliderRoomMaxPlayer.value;
        if (playerCount >= 4 && playerCount <= 16)
        {
            if (title.Replace(" ", "").Length > 0)
            {
                RoomOptions roomOption = new RoomOptions();
                roomOption.MaxPlayers = playerCount;
                roomOption.IsOpen = true;
                roomOption.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                roomOption.CustomRoomProperties.Add("isStart", false);
                roomOption.CustomRoomProperties.Add("Vote", false);
                roomOption.CustomRoomProperties.Add("murders", (int)sliderMurderCount.value);
                roomOption.CustomRoomProperties.Add("moveSpeed", sliderMoveSpeed.value);
                roomOption.CustomRoomProperties.Add("killCooldown", sliderKillCooldown.value);
                roomOption.CustomRoomProperties.Add("nicknameVisible", tglNickname.isOn);
                roomOption.CustomRoomProperties.Add("fallingDamage", tglFallingDamage.isOn);
                roomOption.CustomRoomProperties.Add("startItem", tglStartItem.isOn);
                roomOption.CustomRoomProperties.Add("runable", tglRunable.isOn);

                PhotonNetwork.LeaveLobby();
                PhotonNetwork.CreateRoom(title, roomOption);
            }
            else
            {
                EventManager.SendEvent("PopupMessage", $"방 제목을 입력해주세요.");
            }
        }
        else
        {
            EventManager.SendEvent("PopupMessage", $"플레이어 수 범위를 벗어났습니다.\n범위 : 2 ~ 8");
        }
    }

    // ***** SetNickname ***** //
    private void SetNickname(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        areaInputNickname.SetActive(false);
        areaRoomList.SetActive(true);
    }

    // ***** RefreshRoomList ***** //
    private void RefreshRoomList()
    {
        int i = 0;
        foreach (var room in rooms)
        {
            if (roomListItems.Count <= i)
            {
                RoomListItem item = Instantiate(roomListPrefab, roomListParent).GetComponent<RoomListItem>();
                roomListItems.Add(item);
            }
            i++;
        }

        if (roomListItems.Count > rooms.Count)
        {
            int diff = roomListItems.Count - rooms.Count;
            for (int list = 0; list < diff; list++)
            {
                Destroy(roomListItems[list].gameObject);
            }
            roomListItems.RemoveRange(0, diff);
        }

        i = 0;
        foreach (var room in rooms)
        {
            roomListItems[i].Refresh(i, room.Value);
            i++;
        }
    }

    // ***** RefreshPlayerList ***** //
    private void RefreshPlayerList()
    {
        int i = 0;
        foreach (var player in players)
        {
            playerListItems[i++].Refresh(player.Value);
        }
        for (int list = i; list < playerListItems.Count; list++)
        {
            playerListItems[list].Clear();
        }
    }

    // ***** CheckIsMasterClient ***** //
    private void CheckIsMasterClient()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            btnStart.gameObject.SetActive(true);
            btnReady.gameObject.SetActive(false);
            SetPlayerProperties("isReady", false);
        }
        else
        {
            btnStart.gameObject.SetActive(false);
            btnReady.gameObject.SetActive(true);
        }
        btnCancel.gameObject.SetActive(false);
    }

    // ***** OnStart ***** //
    private void OnStart()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount <= (int)PhotonNetwork.CurrentRoom.CustomProperties["murders"] * 2)
            {
                EventManager.SendEvent("PopupMessage", $"인원이 부족합니다!");
            }
            else
            {
                int noReady = 0;
                foreach (var item in players)
                {
                    if (PhotonNetwork.LocalPlayer.NickName != item.Value.NickName)
                    {
                        if (!(bool)item.Value.CustomProperties["isReady"]) noReady++;
                    }
                }

                if (noReady > 0)
                {
                    EventManager.SendEvent("PopupMessage", $"{noReady}명이 준비를 안했습니다.");
                }
                else
                {
                    int maxCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["murders"];
                    int count = maxCount;
                    bool[] murderIndex = new bool[PhotonNetwork.CurrentRoom.PlayerCount];
                    while (count > 0)
                    {
                        int randomIndex = Random.Range(0, murderIndex.Length);
                        if (!murderIndex[randomIndex])
                        {
                            murderIndex[randomIndex] = true;
                            count--;
                        }
                    }
                    string result = "";

                    int i = 0;
                    foreach (var player in PhotonNetwork.CurrentRoom.Players)
                    {
                        if (murderIndex[i])
                        {
                            var p = player.Value.CustomProperties;
                            p["isMurder"] = true;
                            player.Value.SetCustomProperties(p);
                        }
                        result += $"{murderIndex[i]} ({player.Value.NickName}), ";
                        i++;
                    }
                    Debug.Log(result);
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    SetRoomProperties("isStart", true);
                }
            }
        }
    }

#nullable enable
    // ***** SetPlayerProperties ***** //
    private void SetPlayerProperties(params object?[] v)
    {
        if (v != null)
        {
            var settings = PhotonNetwork.LocalPlayer.CustomProperties;
            if (v.Length > 0 && v.Length % 2 == 0)
            {
                for (int i = 0; i < v.Length / 2; i++)
                {
                    settings[v[i * 2]] = v[i * 2 + 1];
                }
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(settings);
        }
    }

    // ***** SetRoomProperties ***** //
    private void SetRoomProperties(object? key, object? value)
    {
        var options = PhotonNetwork.CurrentRoom.CustomProperties;
        options[key] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(options);
    }
#nullable disable

    #endregion Custom Method
}
