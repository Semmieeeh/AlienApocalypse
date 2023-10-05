using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject prefab;
    public GameObject enemyManager;
    [Space]
    public Transform spawnPoint;
    [Space]
    public GameObject roomCam;
    [Space]
    public GameObject connectingUI;
    public GameObject cantConnectUi;
    public GameObject nicknameUI;
    public string roomNameToJoin = "Test";
    public bool spawnedEnemy = false;
    public RoomList roomList;

    private string nickname = "Unnamed";
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
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
    }
    

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We're in a room!");
        SpawnPlayer();
        gameObject.SetActive(false);

    }


    public void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject enemy = PhotonNetwork.Instantiate(enemyManager.name, spawnPoint.position, Quaternion.identity);

        }
        GameObject player = PhotonNetwork.Instantiate(prefab.name, spawnPoint.position, Quaternion.identity);
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        
    }
    
}
