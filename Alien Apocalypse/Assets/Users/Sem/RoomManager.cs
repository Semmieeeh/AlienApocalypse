using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject prefab;
    [Space]
    public Transform spawnPoint;
    [Space]
    public GameObject roomCam;
    [Space]
    public GameObject connectingUI;
    public GameObject nicknameUI;
    public string roomNameToJoin = "Test";

    private string nickname = "Unnamed";
    void Start()
    {
        
    }
    public void SetNickName(string name)
    {
        nickname = name;
    }

    public void JoinRoomButton()
    {
        Debug.Log("Connecting...");
        nicknameUI.SetActive(false);
        connectingUI.SetActive(true);
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
    }
    

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We're in a room!");
        roomCam.SetActive(false);
        SpawnPlayer();
        
    }

    public void SpawnPlayer()
    {
        GameObject player = PhotonNetwork.Instantiate(prefab.name, spawnPoint.position, Quaternion.identity);
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
    }
}
