using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class RoomItemButton : MonoBehaviour
{
    public string roomName;
    public TextMeshProUGUI text1, text2;

    public void OnButtonPressed()
    {
        RoomList.instance.JoinRoomByName(roomName);
    }
}
