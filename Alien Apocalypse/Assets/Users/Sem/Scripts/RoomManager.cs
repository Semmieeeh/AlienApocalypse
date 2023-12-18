using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject[] prefabs;
    public GameObject enemyManager;
    [Space]
    public Transform spawnPoint;
    [Space]
    public GameObject roomCam;
    [Space]
    public GameObject connectingUI;
    public GameObject cantConnectUi;
    public GameObject nicknameUI;
    public string roomNameToJoin;
    public bool spawnedEnemy = false;
    public RoomList roomList;
    public GameObject island;
    public Transform islandPos;

    public string nickname = "Unnamed";
    void Start()
    {
        spawnPoint = GameObject.Find("PlayerSpawnPoint").transform;
    }
    public void SetNickName(string name)
    {
        nickname = name;
    }

    public void JoinRoomButton()
    {
        Debug.Log("Connecting...");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, options, null);
    }
    

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We're in a room!");
        SpawnPlayer();
        gameObject.SetActive(false);

    }

    public GameObject pointsManager;
    public GameObject vanObj;
    public GameObject chests;
    public void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            //GameObject enemy = Instantiate(enemyManager, Vector3.zero, Quaternion.identity);
            GameObject pointsMan = PhotonNetwork.Instantiate(pointsManager.name,spawnPoint.position, Quaternion.identity);
            GameObject van = PhotonNetwork.Instantiate(vanObj.name, new Vector3(2, 0.4f, -22f),Quaternion.identity);
            GameObject chest = PhotonNetwork.Instantiate(chests.name, new Vector3(0,0,0),Quaternion.identity);

        }
        
        GameObject playerPrefab = PhotonNetwork.Instantiate(prefabs[PhotonNetwork.CurrentRoom.PlayerCount-1].name, spawnPoint.position, spawnPoint.transform.rotation);
        GameObject player = playerPrefab.transform.GetChild(0).gameObject;
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.All, nickname);

    }
    
}
