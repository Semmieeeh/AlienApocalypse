using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Runtime.CompilerServices;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList instance;
    [Header("UI")]
    public Transform roomlistParent;
    public GameObject roomlistButton;
    public GameObject roomManagerGameObject;
    public RoomManager roomManager;
    public List<RoomInfo> rooms = new List<RoomInfo>();
    public void Awake()
    {
        instance = this;
    }
    IEnumerator Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);


        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public void ChangeRoomToCreateName(string roomName)
    {
        roomManager.roomNameToJoin = roomName;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        if (rooms.Count <= 0)
        {
            rooms = roomList;
        }
        else
        {
            foreach (var room in rooms)
            {
                for(int i = 0; i <rooms.Count; i++)
                {
                    if (rooms[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = rooms;


                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }
                        rooms = newList;
                    }
                }
            }
        }

        UpdateUI();
    }



    void UpdateUI()
    {
        foreach(Transform roomItem in roomlistParent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach(var room in rooms)
        {
            GameObject roomItem = Instantiate(roomlistButton, roomlistParent);
            roomItem.GetComponent<RoomItemButton>().roomName = room.Name;
            roomItem.GetComponent<RoomItemButton>().text1.text = room.Name;
            roomItem.GetComponent<RoomItemButton>().text2.text = room.PlayerCount + "/4";

            
        }
    }


    public void JoinRoomByName(string name)
    {
        roomManager.roomNameToJoin = name;
        roomManagerGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
