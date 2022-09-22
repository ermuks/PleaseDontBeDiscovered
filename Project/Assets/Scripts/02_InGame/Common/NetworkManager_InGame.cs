using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class NetworkManager_InGame : MonoBehaviourPunCallbacks
{
    private GameObject myCharacter;
    private bool isOver = false;

    private bool isFinishVote = false;

    //private bool isAlreadyVoted = false;

    private void Awake()
    {
        string path = "Prefabs/Player";
        Transform playerTransform = Resources.Load<GameObject>(path).transform;
        myCharacter = PhotonNetwork.Instantiate(path, playerTransform.position, playerTransform.rotation);
        myCharacter.AddComponent<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked;

        EventManager.AddEvent("InGameData :: PlayerPositionSetting", (p) =>
        {
            GameObject[] areas = GameObject.FindGameObjectsWithTag("SpawnArea");

            GameObject spawnArea = areas[Random.Range(0, areas.Length)];
            Bounds bounds = spawnArea.GetComponent<Collider>().bounds;

            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;
            myCharacter.GetComponent<CharacterController>().enabled = false;
            myCharacter.transform.position = bounds.center + new Vector3(Random.Range(-x, x), Random.Range(-y, y), Random.Range(-z, z));
            myCharacter.GetComponent<CharacterController>().enabled = true;
        });
        EventManager.SendEvent("InGameData :: PlayerPositionSetting");

        EventManager.AddEvent("InGameData :: FinishVoteAnimationPlaying", (p) =>
        {
            isFinishVote = (bool)p[0];
        });
        EventManager.AddEvent("InGameData :: AlreadyVoted", (p) =>
        {
            PhotonNetwork.LocalPlayer.CustomProperties["alreadyVoted"] = p[0];
        });
        EventManager.AddEvent("InGameData :: ClearDeadPlayer", (p) =>
        {
            ClearDeadPlayer();
        });

        EventManager.AddData("InGameData >> FinishVoteAnimationPlaying", (p) => isFinishVote);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        EventManager.SendEvent("InGameUI :: RefreshVotePlayerList");
        if (!isOver && !Settings.instance.isDebug) CheckPlayers();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!isOver && !Settings.instance.isDebug) CheckPlayers();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged != null)
        {
            var properties = propertiesThatChanged;
            if (properties.ContainsKey("Vote"))
            {
                if ((bool)properties["Vote"] && !(bool)EventManager.GetData("InGameUI >> VoteUIActive"))
                {
                    EventManager.SendEvent("InGameUI :: OpenVoteUI");
                }
                if (!(bool)properties["Vote"] && (bool)EventManager.GetData("InGameUI >> VoteUIActive"))
                {
                    EventManager.SendEvent("InGameUI :: EndVote");
                }
            }
        }
    }

    private void ClearDeadPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            Player p = players[i].GetComponent<PhotonView>().Owner;
            if ((bool)p.CustomProperties["isDead"])
            {
                players[i].GetComponent<PlayerData>().DestroyPlayer();
            }
        }
    }

    private void CheckPlayers()
    {
        int surviverCount = 0;
        int murderCount = 0;

        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            var properties = player.CustomProperties;
            if (!(bool)properties["isDead"] && !(bool)properties["isMurder"]) surviverCount++;
            if (!(bool)properties["isDead"] && (bool)properties["isMurder"]) murderCount++;
        }

        if (murderCount >= surviverCount) GameOver(true);
        if (murderCount == 0) GameOver(false);
    }

    private void GameOver(bool murderWin)
    {
        isOver = true;
        EventManager.SendEvent("InGameData :: FinishVoteAnimationPlaying", false);
        EventManager.SendEvent("InGameData :: AlreadyVoted", false);
        PhotonNetwork.Destroy(myCharacter.GetComponent<PhotonView>());
        EventManager.SendEvent("InGameUI :: GameOver", murderWin);
    }
}
