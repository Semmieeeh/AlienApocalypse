using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomItemButton : MonoBehaviour
{
    public string roomName;

    public void OnButtonPressed()
    {
        RoomList.instance.JoinRoomByName(roomName);
    }
}
