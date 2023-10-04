using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class RoomDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI serverName, players, status, connection;

    RoomInfo room;

    public void SetServer (RoomInfo room )
    {
        this.room = room;
        serverName.text = room.Name;
        players.text = $"{room.PlayerCount} / {room.MaxPlayers}";

        status.text = room.IsOpen ? "Available" : "Closed";

        connection.text = Random.Range (5, 34).ToString();
    }
}
